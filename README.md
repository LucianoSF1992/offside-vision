# Offside Vision ⚽

**Plataforma experimental para análise de impedimento no futebol com
.NET, Angular e visão computacional.**

O Offside Vision é um projeto educacional que explora como processamento
de imagens, visão computacional e análise geométrica podem auxiliar na
análise de lances de impedimento.

A proposta é evoluir de um MVP com detecções informadas manualmente para
uma plataforma experimental capaz de detectar jogadores e bola,
acompanhar movimentos e estimar posições de impedimento.

> **Importante:** estar em posição de impedimento não significa, por si
> só, cometer uma infração. Este projeto é experimental e não substitui
> a arbitragem oficial.

## Situação atual

**MVP --- Fase 1**

Funcionalidades implementadas: - Upload de imagem pela interface
Angular. - API REST desenvolvida com ASP.NET Core. - Recebimento de
dados estruturados de jogadores e bola. - Cálculo geométrico de uma
linha de referência. - Identificação de atacantes potencialmente em
posição de impedimento. - Anotação da imagem utilizando OpenCV. -
Integração entre frontend Angular e backend .NET. - Testes automatizados
iniciais das camadas de domínio e aplicação.

**A detecção automática de jogadores e bola por meio de um modelo ONNX
ainda não está implementada.** No MVP atual, as detecções são fornecidas
manualmente para validar o fluxo de análise.

## Tecnologias utilizadas

### Backend

-   C#
-   .NET 8 / ASP.NET Core
-   Clean Architecture
-   API REST
-   Entity Framework Core (estrutura inicial)
-   OpenCvSharp
-   ONNX Runtime (base para futura integração de modelos)
-   xUnit

### Frontend

-   Angular 19
-   TypeScript
-   HTML5 Canvas
-   Angular Material

### Ferramentas e infraestrutura

-   Git e GitHub
-   Docker

## Arquitetura do projeto

A solução está organizada em módulos, separando responsabilidades de
domínio, aplicação, infraestrutura, visão computacional e API.

``` text
OffsideVision/
├── backend/
│   ├── OffsideVision.Api/
│   ├── OffsideVision.Application/
│   ├── OffsideVision.Domain/
│   ├── OffsideVision.Infrastructure/
│   └── OffsideVision.ComputerVision/
├── frontend/
│   └── offside-vision-web/
├── tests/
│   ├── OffsideVision.Domain.Tests/
│   └── OffsideVision.Application.Tests/
├── OffsideVision.sln
└── README.md
```

## Pré-requisitos

-   .NET SDK 8
-   Node.js 20.19 ou versão compatível com Angular 19
-   npm
-   Docker Desktop (opcional)

## Como executar localmente

### 1. Backend

Na raiz do repositório, execute:

``` bash
dotnet restore backend/OffsideVision.Api/OffsideVision.Api.csproj
dotnet run --project backend/OffsideVision.Api/OffsideVision.Api.csproj
```

Endereços locais: - API: `http://localhost:5080` - Swagger:
`http://localhost:5080/swagger` - Verificação de saúde:
`http://localhost:5080/api/health`

### 2. Frontend

Abra outro terminal e execute:

``` bash
cd frontend/offside-vision-web
npm install
npm start
```

Acesse `http://localhost:4200`.

### 3. Testes automatizados

Na raiz do repositório:

``` bash
dotnet test tests/OffsideVision.Domain.Tests/OffsideVision.Domain.Tests.csproj
dotnet test tests/OffsideVision.Application.Tests/OffsideVision.Application.Tests.csproj
```

## Como testar o MVP

1.  Inicie o backend e o frontend.
2.  Acesse `http://localhost:4200`.
3.  Selecione uma imagem de um lance de futebol.
4.  Informe as detecções de demonstração: equipe, identificador, número
    da camisa e coordenadas normalizadas.
5.  Informe a posição horizontal da bola.
6.  Clique em **Analisar lance**.
7.  Consulte o resultado e a imagem anotada retornados pela API.

### Convenção geométrica utilizada

Para esta primeira versão, o eixo X representa a direção do ataque: - Os
valores de X variam de `0` a `1`. - Valores maiores de X representam
posições mais próximas da linha de gol adversária. - A linha de
referência é calculada usando a posição da bola e a posição do segundo
último defensor. - Atacantes com coordenada X maior que a linha
calculada são sinalizados como potencialmente em posição de impedimento.

Cálculo simplificado:

``` csharp
var lineX = Math.Max(ballX, secondLastDefenderX);
```

As coordenadas são relativas à imagem. O MVP ainda não identifica
automaticamente os pontos corporais elegíveis previstos na Regra 11.

## API

**Endpoint:** `POST /api/analyses/image`

**Formato:** `multipart/form-data`

  Campo          Descrição
  -------------- -------------------------------------------
  `image`        Arquivo de imagem do lance
  `detections`   JSON com a posição da bola e os jogadores

Exemplo do conteúdo do campo `detections`:

``` json
{
  "ballX": 0.62,
  "players": [
    {"id": "A17", "team": "Attacking", "shirtNumber": 17, "x": 0.78, "y": 0.45},
    {"id": "D4", "team": "Defending", "shirtNumber": 4, "x": 0.35, "y": 0.35},
    {"id": "D23", "team": "Defending", "shirtNumber": 23, "x": 0.55, "y": 0.58},
    {"id": "D8", "team": "Defending", "shirtNumber": 8, "x": 0.25, "y": 0.72}
  ]
}
```

A API retorna o resultado geométrico da análise e a imagem anotada em
Base64.

## Limitações conhecidas

-   As detecções de jogadores e bola são informadas manualmente.
-   A inferência ONNX depende de um modelo treinado e de um adaptador
    específico para o pré-processamento e a interpretação das saídas.
-   Estimativa de pose corporal ainda não está implementada.
-   O sistema não identifica o instante exato do passe.
-   Rastreamento temporal de jogadores ainda não está implementado.
-   A perspectiva do campo não é calibrada por homografia.
-   O sistema não avalia participação ativa, interferência ou os demais
    critérios necessários para determinar uma infração de impedimento.

Por essas razões, o resultado deve ser interpretado exclusivamente como
uma **referência geométrica experimental**, e não como uma decisão
oficial de impedimento.

## Próximas evoluções

-   [x] Estrutura inicial do backend .NET.
-   [x] Interface Angular.
-   [x] Upload de imagem e integração com a API.
-   [x] Cálculo geométrico da linha de referência.
-   [x] Anotação da imagem com OpenCV.
-   [ ] Adaptador de inferência para modelos ONNX.
-   [ ] Detecção automática de jogadores e bola.
-   [ ] Estimativa de pose corporal.
-   [ ] Rastreamento temporal dos jogadores.
-   [ ] Identificação do momento do passe.
-   [ ] Calibração do campo por homografia.
-   [ ] Evolução das regras de análise conforme a Regra 11.
-   [ ] Histórico e exportação das análises.
-   [ ] Pipeline de CI/CD.

## Objetivo do projeto

O Offside Vision é um projeto pessoal de engenharia de software e
experimentação tecnológica, com foco na integração entre: -
Desenvolvimento backend com C# e .NET. - Desenvolvimento frontend com
Angular e TypeScript. - Processamento de imagens e visão
computacional. - Arquitetura de software e testes automatizados.

**Projeto educacional e experimental. Não destinado a decisões oficiais
de arbitragem.**
