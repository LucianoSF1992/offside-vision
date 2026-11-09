# Arquitetura e decisões

- **Domain**: modelos e cálculo geométrico puro, sem dependência de infraestrutura.
- **Application**: caso de uso de análise e contrato `IImageAnnotator`.
- **Infrastructure**: anotação de imagem com OpenCvSharp.
- **ComputerVision**: prontidão para carregar sessão ONNX; o contrato de tensores ainda depende do modelo escolhido.
- **Api**: endpoints HTTP, validação de upload e composição de dependências.
- **Frontend**: aplicação Angular que envia multipart/form-data e apresenta resultado.

## Regra geométrica implementada
Com X crescente em direção ao gol adversário, o limite da linha é `max(ballX, secondLastDefenderX)`. Um atacante é marcado como potencialmente adiantado se seu X for maior que esse limite.

É uma simplificação do cálculo espacial: não há pose estimation, distinção entre partes do corpo elegíveis, calibração de perspectiva nem sincronização com o instante do toque.
