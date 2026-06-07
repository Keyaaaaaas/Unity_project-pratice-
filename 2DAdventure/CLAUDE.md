# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 2D 横版冒险游戏，使用 **Universal Render Pipeline (URP)** 和 **Unity Input System 1.7**。

## Key Architecture

### 角色系统 (Character System)
- **`Character`** (`Assets/Scripts/General/Character.cs`) — 所有角色的基类。管理生命值 (`maxHealth`/`currentHealth`)、无敌状态 (`invulnerableDuration`/`inInvulnerable`)。通过 `UnityEvent<Transform> OnTakeDamage` 和 `UnityEvent OnDead` 向外广播事件。
- **`PlayerController`** (`Assets/Scripts/Player/PlayerController.cs`) — 挂载在玩家 GameObject 上，持有 `Rigidbody2D`、`PhysicsCheck`、`PlayerAnimation`。在 `Awake()` 中初始化 `PlayerInputControl` 并绑定 `Jump`/`Run`/`Attack` 事件的回调委托。通过 `FixedUpdate()` 驱动 `Move()`。朝向通过 `transform.localScale.x` 的正负来控制。

### 输入系统 (Input System)
- **`PlayerInputControl`** (`Assets/Settings/input System/PlayerInputControl.cs`) — 由 `.inputactions` 文件自动生成的 C# 包装类。定义了两个 ActionMap：`Gameplay`（Move/Look/Fire/Jump/Run/Attack）和 `UI`。
- 输入绑定：WASD/方向键/手柄左摇杆 → Move，Space → Jump，Shift → Run，J → Attack。
- **不要在 `.inputactions.cs` 中手动修改代码**，改动应在 `.inputactions` 资产文件中进行。

### 物理检测 (Physics Check)
- **`PhysicsCheck`** — 用 `Physics2D.OverlapCircle` 检测地面。`bottomOffset` + `checkRadius` 定义检测区域，`groundLayer` 过滤碰撞层。通过 `isGround` bool 向外暴露结果。

### 攻击系统 (Attack System)
- **`Attack`** — 挂在攻击碰撞体上。通过 `OnTriggerStay2D` 检测碰撞，调用对方 `Character.TakeDamage(this)`，传递自身引用（包含 `damage`、`attackRange`、`attackRate`）。
- 攻击流程：玩家按 J → `PlayerController.Attack()` → `isAttack = true` → `PlayerAnimation.PlayerAttack()` 触发 animator 的 "attack" trigger → 动画播放 → `AttackAnimation.OnStateExit()` 重置 `isAttack = false`。

### 动画系统 (Animation System)
- **`PlayerAnimation`** — 每帧从 `Rigidbody2D`、`PhysicsCheck`、`PlayerController` 读取状态并设置 Animator 参数 (`velocityX`/`velocityY`/`isGround`/`isDead`/`isAttack`)。
- **`AttackAnimation`** / **`HurtAnimation`** — 都是 `StateMachineBehaviour`，在 `OnStateExit` 时重置 `PlayerController` 的 `isAttack`/`isHurt` 标志位。
- **Animator Controller** 位于 `Assets/Animations/Player/Player.controller`，动画片段以 `blue` 前缀命名（`blueidle`/`bluerun`/`blueattack1-3`/`bluehurt`/`bluehurt2`/`bluejump1-4`/`blueland`/`bluedead`）。

### 目录结构
```
Assets/
├── Animations/Player/     # Animator Controller + 动画片段
├── Scenes/                # SampleScene.unity
├── Scripts/
│   ├── General/           # Character.cs, PhysicsCheck.cs, Attack.cs
│   └── Player/            # PlayerController, PlayerAnimation, AttackAnimation, HurtAnimation
├── Settings/
│   ├── input System/      # PlayerInputControl.inputactions + 生成的 .cs
│   ├── Physics Material/  # Normal.physicsMaterial2D
│   ├── UniversalRP.asset  # URP 管线配置
│   └── Renderer2D.asset   # 2D 渲染器配置
├── TileMap/               # Tilemap 资源
├── Legacy-Fantasy - High Forest 2.0/  # 第三方地形资源包
└── generic_char_v0.2/     # 角色 Sprite 资源
```

## Code Conventions

- **命名**: `PascalCase` 类名和方法，`camelCase` 字段。`[Header("中文标签")]` 在 Inspector 中分组参数。
- **引用获取**: 组件引用统一在 `Awake()` 中通过 `GetComponent<>()` 获取，而非在 Inspector 中拖拽赋值。
- **输入事件**: 使用 `InputAction.CallbackContext` 委托模式绑定（`started`/`canceled`），不轮询 `Input.GetKey`。
- **中文注释**: 代码注释和 Header 标签使用中文。

## Build & Development

- **Unity 版本**: 项目使用 Unity 6（从 URP 16.0.6 / Input System 1.7 推断）
- **目标平台**: 当前未指定，标准 PC 平台
- **场景入口**: `Assets/Scenes/SampleScene.unity`
- **运行**: 在 Unity Editor 中打开场景后点击 Play
- **无自动化测试** 或 CI/CD 配置

## Important Notes

- `PlayerInputControl.cs` 是自动生成的，不要手动编辑——请修改 `.inputactions` 文件后让 Unity 重新生成。
- 玩家朝向左/右是通过 `transform.localScale.x` 的 ±1 切换实现的，不要改用 `flipX` 或其他方式。
- `Character` 的生命值逻辑中，死亡条件为 `currentHealth - attacker.damage <= 0` 时设置 `currentHealth = 0` 并触发 `OnDead`，但**没有销毁 GameObject**——需要后续在 `OnDead` 事件监听中处理。
- `PlayerController` 中的 `Run` 方法硬编码了 `speed = 600`，后续应改为可配置参数。
