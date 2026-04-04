### 4.2 Level Scene (Gameplay)
Assets can:
- Background mine (co the doi theo level)
- Miner sprite
- Hook + Rope
- Item sprites: Gold, Stone, Diamond
- UI: Score, Timer, Target, Inventory power-up, Result panel

Scripts can:
- `LevelManager.cs`
- `HookController.cs`
- `ItemController.cs`
- `ScoreManager.cs`
- `TimerManager.cs`
- `WeightCalculator.cs`
- `PowerUpController.cs`
- `UIController.cs`
- `ResultPanel.cs` (hoac dung `EndLevelUI.cs` va doi ten logic)

#### LevelManager.cs
Attributes:
- `int currentLevel`
- `int targetScore`
- `float levelDuration = 60f`
- `GameObject level1Prefab`
- `GameObject level2Prefab`
- `GameObject level3Prefab`
- `Transform levelRoot`

Logic:
- Doc `currentLevel` tu `GameStateManager`
- Spawn prefab dung level
- Setup target score theo level
- Lang nghe su kien het gio tu `TimerManager`
- Xu ly pass/fail:
  - Pass:
    - Show text: "ban da len cap do tiep"
    - Neu `currentLevel < 3`: delay ngan roi load Shop
    - Neu `currentLevel == 3`: delay ngan roi load MainMenu
  - Fail:
    - Show text: "ban chua hoan thanh nhiem vu"
    - Hien nut ve MainMenu

#### HookController.cs
Attributes:
- `float rotateSpeed`
- `float minAngle`
- `float maxAngle`
- `float launchSpeed`
- `float baseRetractSpeed`
- `bool isLaunched`
- `ItemController attachedItem`

Logic:
- Swing trai-phai lien tuc
- Nhan input de phong hook
- Bat item qua collision
- Keo ve voi toc do bi anh huong boi trong luong

#### ItemController.cs
Attributes:
- `ItemType itemType` (Gold, Stone, Diamond)
- `int value`
- `float weight`
- `bool isHooked`

Logic:
- Khi bi hook trung -> attach vao hook
- Khi ve den vi tri collect -> cong diem va destroy/deactivate item

#### WeightCalculator.cs
Attributes:
- `float baseSpeed`
- `float weightFactor`
- `bool strengthActive`

Logic:
- Cong thuc toc do keo: `speed = baseSpeed / (1 + totalWeight * weightFactor)`
- Neu strength active -> bo qua weight

#### PowerUpController.cs
Attributes:
- `int bombCount`
- `int strengthCount`
- `int timeBoostCount`
- `float strengthDuration`

Logic:
- Bom: xoa item dang keo
- Strength: trong 1 khoang thoi gian bo qua weight
- Time Boost: +10s vao timer

#### ScoreManager.cs
Attributes:
- `int currentScore`
- `int targetScore`

Logic:
- Cong diem theo item
- Kiem tra dat target hay chua

#### TimerManager.cs
Attributes:
- `float timeRemaining = 60f`
- `bool isRunning`

Logic:
- Dem nguoc den 0
- Trigger event khi het gio

#### ResultPanel.cs
Attributes:
- `GameObject panel`
- `TMP_Text messageText`
- `Button backToMenuButton`

Logic:
- `ShowPass()`: text "ban da len cap do tiep"
- `ShowFail()`: text "ban chua hoan thanh nhiem vu", bat nut ve menu

Animation/SFX:
- Hook swing, collect popup, panel fade
- Music-Level, dig/pull/collect sounds

---
