# Offside Vision ⚽

**An experimental football offside analysis platform built with .NET,
Angular and computer vision.**

Offside Vision is an educational project exploring how image processing,
computer vision and geometric analysis can support the analysis of
football offside situations.

The goal is to evolve from an MVP that uses manually supplied detections
into an experimental platform capable of detecting players and the ball,
tracking movement and estimating offside positions.

> **Important:** being in an offside position does not automatically
> mean committing an offside offence. This project is experimental and
> does not replace official refereeing decisions.

## Current Status

**MVP --- Phase 1**

Implemented features:

-   Image upload through the Angular interface.
-   REST API developed with ASP.NET Core.
-   Structured input for player and ball detections.
-   Geometric reference-line calculation.
-   Identification of attacking players potentially in an offside
    position.
-   Image annotation using OpenCV.
-   Integration between the Angular frontend and .NET backend.
-   Initial automated tests for the domain and application layers.

**Automatic player and ball detection using an ONNX model has not yet
been implemented.** In the current MVP, detections are provided manually
to validate the analysis workflow.

## Technology Stack

### Backend

-   C#
-   .NET 8 / ASP.NET Core
-   Clean Architecture
-   REST API
-   Entity Framework Core (initial foundation)
-   OpenCvSharp
-   ONNX Runtime (foundation for future model integration)
-   xUnit

### Frontend

-   Angular 19
-   TypeScript
-   HTML5 Canvas
-   Angular Material

### Tools and Infrastructure

-   Git and GitHub
-   Docker

## Project Architecture

The solution is organized into modules with separate responsibilities
for the domain, application, infrastructure, computer vision and API
layers.

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

## Prerequisites

-   .NET SDK 8
-   Node.js 20.19 or a version compatible with Angular 19
-   npm
-   Docker Desktop (optional)

## Run Locally

### 1. Backend

From the repository root, run:

``` bash
dotnet restore backend/OffsideVision.Api/OffsideVision.Api.csproj
dotnet run --project backend/OffsideVision.Api/OffsideVision.Api.csproj
```

Local endpoints:

-   API: `http://localhost:5080`
-   Swagger: `http://localhost:5080/swagger`
-   Health check: `http://localhost:5080/api/health`

### 2. Frontend

Open another terminal and run:

``` bash
cd frontend/offside-vision-web
npm install
npm start
```

Open `http://localhost:4200`.

### 3. Automated Tests

From the repository root:

``` bash
dotnet test tests/OffsideVision.Domain.Tests/OffsideVision.Domain.Tests.csproj
dotnet test tests/OffsideVision.Application.Tests/OffsideVision.Application.Tests.csproj
```

## How to Test the MVP

1.  Start the backend and frontend.
2.  Open `http://localhost:4200`.
3.  Select an image of a football situation.
4.  Provide demonstration detections: team, identifier, shirt number and
    normalized coordinates.
5.  Provide the ball's horizontal position.
6.  Click **Analyze Play**.
7.  Review the analysis result and annotated image returned by the API.

### Geometric Convention

In this first version, the X axis represents the attacking direction:

-   X coordinates range from `0` to `1`.
-   Larger X values represent positions closer to the opponent's goal
    line.
-   The reference line is calculated using the ball position and the
    second-last defender's position.
-   Attacking players with an X coordinate greater than the calculated
    line are flagged as potentially in an offside position.

Simplified calculation:

``` csharp
var lineX = Math.Max(ballX, secondLastDefenderX);
```

Coordinates are relative to the image. The MVP does not yet
automatically identify the eligible body parts defined by Law 11.

## API

**Endpoint:** `POST /api/analyses/image`

**Content type:** `multipart/form-data`

  Field          Description
  -------------- -----------------------------------------------
  `image`        Image file of the football situation
  `detections`   JSON containing the ball position and players

Example value for the `detections` field:

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

The API returns the geometric analysis result and the annotated image
encoded in Base64.

## Known Limitations

-   Player and ball detections are supplied manually.
-   ONNX inference requires a trained model and a specific adapter for
    preprocessing and output interpretation.
-   Body pose estimation is not implemented yet.
-   The system does not identify the exact moment of the pass.
-   Temporal player tracking is not implemented yet.
-   The field perspective is not calibrated using homography.
-   The system does not evaluate active involvement, interference or the
    other criteria required to determine an offside offence.

For these reasons, the output must be interpreted exclusively as an
**experimental geometric reference**, not as an official offside
decision.

## Roadmap

-   [x] Initial .NET backend architecture.
-   [x] Angular interface.
-   [x] Image upload and API integration.
-   [x] Geometric reference-line calculation.
-   [x] Image annotation with OpenCV.
-   [ ] ONNX model inference adapter.
-   [ ] Automatic player and ball detection.
-   [ ] Body pose estimation.
-   [ ] Temporal player tracking.
-   [ ] Pass moment identification.
-   [ ] Field calibration using homography.
-   [ ] Further rule analysis based on Law 11.
-   [ ] Analysis history and export.
-   [ ] CI/CD pipeline.

## Project Goals

Offside Vision is a personal software engineering and technology
exploration project focused on the integration of:

-   Backend development with C# and .NET.
-   Frontend development with Angular and TypeScript.
-   Image processing and computer vision.
-   Software architecture and automated testing.

**Educational and experimental project. Not intended for official
refereeing decisions.**
