# 🤖 Guerra dos Robôs Nordestinos — Captura de Bandeira 3D (Unity)

Desafio de projeto **DIO – Trilha Unity**. Em vez de só mexer num Microgame, o projeto foi expandido para um jogo completo de
**captura de bandeira em 3D, 4x4 robôs, com controles para celular**, arena no sertão e falas em **nordestinês bem-humorado**.

## 🎮 Como se joga
- Dois times: **VERMELHO** (você) e **VERDE** (IA). Cada time tem 4 robôs, um de cada tipo.
- Pegue a **bandeira do lado oposto** e leve até a **sua base**.
- Partida de **5:00 em contagem regressiva**.
- **Vitória:** time com **mais kills**. Se empatar, vence quem **resgatou a bandeira primeiro**.
  Ou seja: quem pega a bandeira primeiro mas termina com menos kills **perde**.
- Robôs mortos renascem em 4 s na própria base. Bandeira derrubada volta sozinha após 12 s.
- **Buracos** no chão fazem o robô **tropeçar** (fica caído ~1,6 s).
- **Lasers saem dos dois olhos** e convergem no ponto da **mira** (centro da tela).

## 🤖 Os 4 robôs
| Robô | Tipo | Especial |
|---|---|---|
| Zé da Peixeira | Bípede humanoide | **Peixeirada**: golpe de faca na frente (50 de dano) |
| Caramelo Arretado | Quadrúpede canino | **Mijada no poste**: poça que atrasa e queima inimigos (maior perto de postes) |
| Lula Lampião | Cefalópode | **Jato de tinta**: nuvem que cega (tela escurece), atrasa e causa dano |
| Camaleão Mandacaru | Réptil camaleão | **Camuflagem**: some da vista dos inimigos por 6 s |

## 📱 Controles
| Ação | Celular | PC (teste no Editor) |
|---|---|---|
| Mover | Joystick à esquerda | WASD / setas |
| Mirar | Arrastar o dedo na metade direita | Botão direito do mouse + mover |
| Laser | Botão **LASER** (segurar) | Espaço |
| Especial | Botão azul | E |

## 🛠️ Como abrir
1. Unity 2021.3+ (testado em design para 2022/6, URP ou Built-in). Pacote `com.unity.ugui` (já vem nos templates).
2. Copie esta pasta para dentro do seu projeto (`Assets/RoboFlagWars`).
3. Menu **Robo Flag Wars → 1 - Criar cena do jogo** (cria `Assets/Scenes/RoboFlagWars.unity`).
4. Menu **Robo Flag Wars → 2 - Configurar projeto para celular** (paisagem + Linear).
5. Em *Project Settings → Player → Active Input Handling* escolha **Both**.
6. Aperte **Play**, escolha seu robô e jogue.
7. Para Android: *File → Build Settings → Android → Build*.

> Em build, adicione os shaders usados (Universal Render Pipeline/Lit ou Standard, Skybox/Procedural, Sprites/Default)
> em *Project Settings → Graphics → Always Included Shaders*.

## 🧱 Arquitetura (Assets/RoboFlagWars/Scripts)
- `GameManager` – partida, cronômetro, placar, regras de vitória.
- `ArenaBuilder` – arena do sertão, bases, buracos, postes, cactos, luz e neblina.
- `RobotBuilder` – cria os 4 robôs (PBR) ou usa seus prefabs em `Resources/Robots/<Tipo>.prefab`
  (filhos `EyeL`, `EyeR` e `Knife`).
- `RobotController` – vida, movimento, lasers dos olhos, especiais, tropeço, respawn.
- `RobotAI` – IA (atacantes e defensores).
- `PlayerInput` + `CameraRig` – câmera 3ª pessoa e mira por raycast central.
- `Flag`, `AbilityZone`, `Dialogues`, `SpeechBubble`, `GameUI`.

## ✨ Deixando ainda mais realista
- Troque as primitivas por modelos (Asset Store/Mixamo) com prefabs em `Resources/Robots`.
- URP: ative **Bloom, SSAO, Tonemapping e Color Adjustments** em um Volume global; HDR e MSAA 4x.
- Adicione texturas PBR (Poly Haven) no chão e nos muros; sons e partículas de laser.

## 📄 Texto sugerido de entrega (DIO)
Expandi o desafio em um jogo completo de captura de bandeira 3D para celular, com 4 robôs de habilidades distintas,
IA, cronômetro de 5 min, regras de vitória por kills/bandeira, buracos que fazem tropeçar, lasers pelos olhos com mira
do usuário e falas em nordestinês. Todo o conteúdo é gerado por código C# em Unity.

## 📌 Status e melhorias futuras
Projeto desenvolvido como desafio de aprendizado. Já funcionam: arena, 4 robôs com habilidades, IA, cronômetro,
placar, bandeiras, buracos e lasers pelos olhos.

Pontos a melhorar nas próximas versões:
- Suavizar a câmera 3ª pessoa (seguimento, colisão e sensibilidade do olhar).
- Ajustar o balanceamento e a sensação de controle (velocidade, aceleração, dano e recarga).
- Trocar as primitivas por modelos 3D e animações reais.
- Adicionar sons, partículas e pós-processamento (Bloom/SSAO).
- Testar e otimizar a build para Android e WebGL.
