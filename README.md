# 🤖 Guerra dos Robôs Nordestinos — Captura de Bandeira 3D (Unity)

Desafio de projeto **DIO – Trilha Unity**. Em vez de só modificar um Microgame, o projeto foi expandido para um jogo completo de
**captura de bandeira em 3D, 4x4 robôs**, com arena no sertão, controles de toque pensados para celular e falas em **nordestinês bem-humorado**.

> **Versão testada:** Unity 6.6 (`6000.6.4f1`), template **Universal 3D (URP)**, jogando no Editor (PC).
> Os controles de toque estão implementados, mas ainda não foram testados em um aparelho Android.

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
| Ação | Celular (na tela) | PC (teste no Editor) |
|---|---|---|
| Mover | Joystick à esquerda | WASD / setas |
| Mirar | Arrastar o dedo na metade direita | Botão direito do mouse + mover |
| Laser | Botão **LASER** (segurar) | Espaço |
| Especial | Botão azul | E |

## 🛠️ Como abrir e jogar
1. Instale o **Unity Hub** e o Editor **6000.6.4f1** (ou próximo).
2. Clone o repositório:
```bash
   git clone https://github.com/josewagnerbljr-sys/unity-guerra-robos-bandeira.git
```
3. No Unity Hub: **Adicionar → Adicionar projeto do disco** e escolha a pasta clonada.
4. Em *Edit → Project Settings → Player → Other Settings → Active Input Handling*, escolha **Both**.
5. Menu **Robo Flag Wars → 1 - Criar cena do jogo** (cria `Assets/Scenes/RoboFlagWars.unity`).
6. Menu **Robo Flag Wars → 2 - Configurar projeto para celular** (paisagem + Linear).
7. Aperte **Play**, escolha seu robô e jogue.
8. Para Android: *File → Build Settings → Android → Build*.

> Em builds (Android/WebGL), adicione os shaders usados (Universal Render Pipeline/Lit, Skybox/Procedural, Sprites/Default)
> em *Project Settings → Graphics → Always Included Shaders*.

## 🧱 Arquitetura (`Assets/RoboFlagWars/Scripts`)
- `GameManager` – partida, cronômetro, placar, regras de vitória.
- `ArenaBuilder` – arena do sertão, bases, buracos, postes, cactos, luz e neblina.
- `RobotBuilder` – cria os 4 robôs (materiais PBR) ou usa prefabs em `Resources/Robots/<Tipo>.prefab`
  (filhos `EyeL`, `EyeR` e `Knife`).
- `RobotController` – vida, movimento, lasers dos olhos, especiais, tropeço, respawn.
- `RobotAI` – IA (atacantes e defensores).
- `PlayerInput` + `CameraRig` – câmera 3ª pessoa e mira por raycast central.
- `Flag`, `AbilityZone`, `Dialogues`, `SpeechBubble`, `GameUI`.

## 📌 Status e melhorias futuras
Projeto desenvolvido como desafio de aprendizado. Já funcionam: arena, 4 robôs com habilidades, IA, cronômetro,
placar, bandeiras, buracos e lasers pelos olhos.

Pontos a melhorar nas próximas versões:
- Suavizar a câmera 3ª pessoa (seguimento, colisão e sensibilidade do olhar).
- Ajustar o balanceamento e a sensação de controle (velocidade, aceleração, dano e recarga).
- Trocar as primitivas por modelos 3D e animações reais.
- Adicionar sons, partículas e pós-processamento (Bloom/SSAO).
- Testar e otimizar a build para Android e WebGL.

## 📄 Resumo da entrega
Jogo 3D de captura de bandeira em Unity (C#, URP) com 4 robôs de habilidades distintas, IA, cronômetro de 5 minutos,
regras de vitória por kills e bandeira, buracos que fazem tropeçar, lasers pelos olhos com mira do usuário e falas em
nordestinês. Arena, robôs, materiais e interface são gerados por código.

## 📘 Manual completo
Controles, regras, robôs, como baixar, jogar e resolver problemas: veja o [MANUAL.md](MANUAL.md).

---
Documentação por **Consultoria & Mentoria Blanco**.
