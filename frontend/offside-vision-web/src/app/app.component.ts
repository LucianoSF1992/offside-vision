import { Component, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

type Player = { id: string; team: 'Attacking' | 'Defending'; shirtNumber: number; x: number; y: number };
type ApiResponse = { result: { status: string; explanation: string; offsideLineX: number; secondLastDefenderId: string; secondLastDefenderNumber: number; potentiallyOffsidePlayers: string[] }; annotatedImage: string };

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <main class="shell">
    <header><div class="brand-mark">OV</div><div><h1>Offside Vision</h1><p>Análise experimental de impedimento por visão computacional</p></div><span class="tag">MVP · FASE 1</span></header>
    <section class="intro"><div><span class="eyebrow">FOOTBALL ANALYTICS</span><h2>Transforme um frame em evidências visuais.</h2><p>Envie uma imagem e informe detecções para calcular e projetar uma referência geométrica de impedimento.</p></div></section>
    <section class="workspace">
      <article class="panel">
        <h3>01 / Imagem do lance</h3>
        <label class="upload"><input type="file" accept="image/png,image/jpeg,image/webp" (change)="onFile($event)"><span>＋</span><b>{{ fileName || 'Selecionar imagem' }}</b><small>PNG, JPG ou WEBP · máximo 10 MB</small></label>
        <div class="preview" *ngIf="previewUrl"><img [src]="result?.annotatedImage || previewUrl" alt="Frame analisado"></div>
        <div class="hint">A linha é desenhada pelo backend usando as coordenadas informadas abaixo.</div>
      </article>
      <article class="panel">
        <h3>02 / Detecções de demonstração</h3>
        <p class="muted">Coordenadas normalizadas: X cresce em direção ao gol adversário; Y vai de cima para baixo.</p>
        <div class="field"><label>Posição X da bola (0–1)</label><input type="number" min="0" max="1" step=".01" [(ngModel)]="ballX"></div>
        <div class="player-list">
          <div class="player-row" *ngFor="let p of players; let i = index">
            <span class="dot" [class.blue]="p.team === 'Attacking'"></span>
            <input aria-label="ID" [(ngModel)]="p.id">
            <select [(ngModel)]="p.team"><option value="Attacking">Atacante</option><option value="Defending">Defensor</option></select>
            <input aria-label="Número" type="number" [(ngModel)]="p.shirtNumber">
            <input aria-label="X" type="number" min="0" max="1" step=".01" [(ngModel)]="p.x">
            <button class="remove" (click)="players.splice(i,1)" title="Remover">×</button>
          </div>
        </div>
        <button class="secondary" (click)="addPlayer()">＋ Adicionar jogador</button>
        <button class="primary" [disabled]="!file || loading" (click)="analyze()">{{ loading ? 'Analisando...' : 'Analisar lance →' }}</button>
        <p class="error" *ngIf="error">{{ error }}</p>
      </article>
    </section>
    <section class="panel result" *ngIf="result">
      <div><span class="eyebrow">RESULTADO</span><h2>Posição sob análise</h2><p>{{ result.result.explanation }}</p></div>
      <div class="metrics">
        <div><small>LINHA X</small><strong>{{ result.result.offsideLineX | number:'1.2-2' }}</strong></div>
        <div><small>SEGUNDO-ÚLTIMO DEFENSOR</small><strong>#{{ result.result.secondLastDefenderNumber }} · {{ result.result.secondLastDefenderId }}</strong></div>
        <div><small>ATACANTES À FRENTE</small><strong>{{ result.result.potentiallyOffsidePlayers.length }}</strong></div>
      </div>
      <p class="warning">A posição de impedimento não significa, por si só, infração. O momento do passe e a participação ativa precisam ser avaliados.</p>
    </section>
    <footer>OFFSIDE VISION · Projeto educacional e experimental · Não substitui a decisão oficial da arbitragem.</footer>
  </main>`
})
export class AppComponent {
  @ViewChild('canvas') canvas?: ElementRef<HTMLCanvasElement>;
  file: File | null = null;
  fileName = '';
  previewUrl: string | null = null;
  loading = false;
  error = '';
  result: ApiResponse | null = null;
  ballX = 0.62;
  players: Player[] = [
    { id: 'A17', team: 'Attacking', shirtNumber: 17, x: 0.78, y: 0.45 },
    { id: 'D4', team: 'Defending', shirtNumber: 4, x: 0.35, y: 0.35 },
    { id: 'D23', team: 'Defending', shirtNumber: 23, x: 0.55, y: 0.58 },
    { id: 'D8', team: 'Defending', shirtNumber: 8, x: 0.25, y: 0.72 }
  ];
  constructor(private http: HttpClient) {}
  onFile(event: Event) {
    const input = event.target as HTMLInputElement;
    const f = input.files?.[0];
    this.error = ''; this.result = null;
    if (!f) return;
    if (f.size > 10 * 1024 * 1024) { this.error = 'A imagem deve ter no máximo 10 MB.'; return; }
    this.file = f; this.fileName = f.name;
    const reader = new FileReader();
    reader.onload = () => this.previewUrl = String(reader.result);
    reader.readAsDataURL(f);
  }
  addPlayer() {
    this.players.push({ id: `P${this.players.length + 1}`, team: 'Defending', shirtNumber: 0, x: 0.3, y: 0.5 });
  }
  analyze() {
    if (!this.file) return;
    this.loading = true; this.error = ''; this.result = null;
    const payload = { ballX: Number(this.ballX), players: this.players.map(p => ({ ...p, x: Number(p.x), y: Number(p.y), shirtNumber: Number(p.shirtNumber) })) };
    const form = new FormData();
    form.append('image', this.file);
    form.append('detections', JSON.stringify(payload));
    this.http.post<ApiResponse>('http://localhost:5080/api/analyses/image', form).subscribe({
      next: response => { this.result = response; this.loading = false; },
      error: err => { this.error = err.error?.error || 'Não foi possível conectar à API. Verifique se o backend está em execução.'; this.loading = false; }
    });
  }
}
