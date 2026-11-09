# Offside Vision

MVP educacional para análise visual de impedimento no futebol. O projeto separa posição de impedimento de infração de impedimento e não substitui a arbitragem oficial.

## Stack
- .NET 8 / ASP.NET Core, Clean Architecture, REST
- OpenCvSharp para leitura e anotação de imagens
- ONNX Runtime como base para integração de modelos de detecção
- Angular + TypeScript + HTML Canvas
- xUnit
- Docker

## Pré-requisitos
- .NET SDK 8
- Node.js 20.19+ (ou versão compatível com Angular 19) e npm
- Docker Desktop (opcional)

## Executar localmente

### 1. Backend
No terminal, na raiz do repositório:

```bash
dotnet restore backend/OffsideVision.Api/OffsideVision.Api.csproj
dotnet run --project backend/OffsideVision.Api/OffsideVision.Api.csproj
```

A API inicia em `http://localhost:5080` e o Swagger em `http://localhost:5080/swagger`.

### 2. Frontend
Em outro terminal:

```bash
cd frontend/offside-vision-web
npm install
npm start
```

Abra `http://localhost:4200`.

### 3. Testes
```bash
dotnet test tests/OffsideVision.Domain.Tests/OffsideVision.Domain.Tests.csproj
dotnet test tests/OffsideVision.Application.Tests/OffsideVision.Application.Tests.csproj
```

## Como testar
1. Abra o frontend.
2. Selecione uma imagem de lance (JPG, PNG ou WEBP).
3. Informe as detecções de demonstração: equipe, número, coordenada X normalizada (0 a 1) e coordenada Y (0 a 1).
4. Clique em **Analisar lance**.
5. O backend calcula a referência da linha a partir da posição da bola e do segundo-último defensor, retorna o resultado e a imagem anotada.

**Convenção geométrica do MVP:** o eixo X representa a direção do ataque; valores maiores de X representam posições mais próximas da linha de gol adversária. As coordenadas devem ser informadas em relação à imagem. Para fins demonstrativos, usa-se o ponto corporal elegível mais próximo da linha de gol; no MVP, esse ponto é representado pelo X informado para cada jogador.

## API
`POST /api/analyses/image` (`multipart/form-data`)
- `image`: arquivo de imagem
- `detections`: JSON com `players` e `ballX`
- `players`: `id`, `team` (`Attacking` ou `Defending`), `shirtNumber`, `x`, `y`

Exemplo de `detections`:
```json
{
  "ballX": 0.62,
  "players": [
    {"id":"A17","team":"Attacking","shirtNumber":17,"x":0.78,"y":0.45},
    {"id":"D4","team":"Defending","shirtNumber":4,"x":0.35,"y":0.35},
    {"id":"D23","team":"Defending","shirtNumber":23,"x":0.55,"y":0.58},
    {"id":"D8","team":"Defending","shirtNumber":8,"x":0.25,"y":0.72}
  ]
}
```

## Limitações conhecidas
- O MVP recebe detecções estruturadas para validar o pipeline; a inferência automática ONNX ainda depende de um modelo treinado e de um adaptador específico para seu pré-processamento/saídas.
- Não identifica automaticamente o instante do passe, participação ativa, interferência, defesa deliberada ou reinícios.
- Não calibra a perspectiva do campo. Coordenadas normalizadas em imagem não equivalem a coordenadas reais do gramado.
- Portanto, a saída é uma **referência geométrica experimental**, não uma decisão oficial de impedimento.

## Próximas evoluções
1. Adaptador de modelo ONNX (detecção de pessoa/bola e pose).
2. Tracking temporal e identificação do toque na bola.
3. Calibração do campo por homografia.
4. Motor completo da Regra 11 e interface de revisão manual.
5. Histórico, exportação e CI/CD.
