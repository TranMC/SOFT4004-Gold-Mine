# Kế Hoạch Chi Tiết: Gold Mine Game - Thu Thập Vàng 2D

## Thông Tin Dự Án
- **Scope**: 3 levels nối tiếp, có shop giữa các levels
- **Thời gian/level**: 60 giây
- **Items**: Vàng (1x weight), Đá (2x weight), Kim Cương (3x weight)
- **Power-ups**: Bom (xóa item hiện tại), Strength (bỏ qua trọng lượng), Time Boost (+10s)
- **Assets**: Tìm sprites từ ngoài

---

## 1. SCENES CẦN THIẾT

### 1.1 MainMenu Scene
**Assets cần:**
- Background image (menu background)
- Nút "Play" 
- Nút "Shop"
- Nút "Settings" (tùy chọn)
- Logo game
- Music background

**Scripts cần:**
- `MenuManager.cs` (quản lý UI menu, xử lý click nút)

**Logic:**
- Nút Play → đến LevelSelect
- Nút Shop → đến Shop
- Nút Settings → Settings Panel (hoặc skip)

**Animations**: Fade in menu, button hover effect

**Audio**: Music-Menu.ogg (đã có)

---

### 1.2 LevelSelect Scene
**Assets cần:**
- Background image
- 3 Level select buttons
- Level preview/thumbnail (tùy chọn)
- Back button

**Scripts cần:**
- `LevelSelectManager.cs` (quản lý chọn level)

**Logic:**
- Click level 1, 2, 3 → truyền levelIndex đến Level scene
- Back button → return MainMenu
- Show stars/completion status nếu có (tùy chọn)

**Audio**: SFX_Button.wav (khi click)

---

### 1.3 Level Scene (Gameplay)
**Assets cần:**
- Background image (đất/mine)
- Sprites:
  - Miner character
  - Rope/cable chain
  - Hook/lure (cái móc)
  - Gold item
  - Stone item
  - Diamond item
  - Bomb power-up
  - Strength potion
  - Time boost item
  - Obstacle/rock (nếu có)
- UI Canvas:
  - Score text
  - Timer text
  - Current weight indicator
  - Power-ups inventory
  - Pause button
  - Win/Lose panel

**Scripts cần:**
1. **LevelManager.cs** - Quản lý state level (playing, paused, win, lose)
   - Start time (60s)
   - Check win condition (target score)
   - Handle pause/resume
   
2. **HookController.cs** - Điều khiển cái móc
   - Rotation (360 độ từ trái sang phải)
   - Launch force
   - Retract mechanism
   - Collision detection với items
   - Catch count
   
3. **ItemController.cs** - Controller cho mỗi item
   - Item type (Gold/Stone/Diamond)
   - Weight value
   - Drag speed calculation (based on weight)
   - Attached to hook state
   - Drop behavior
   
4. **PowerUpController.cs** - Xử lý power-ups
   - Bomb: xóa item đang kéo
   - Strength: ignore weight trong 5-10 giây
   - TimeBoost: +10s time
   - UI update
   
5. **ScoreManager.cs** - Quản lý điểm
   - Item value (Gold=10, Stone=5, Diamond=30)
   - Current score display
   - Win condition check
   
6. **TimerManager.cs** - Quản lý thời gian
   - Countdown 60s
   - Display format (MM:SS)
   - Game over khi 0s
   
7. **WeightCalculator.cs** - Tính toán trọng lượng
   - Current weight = sum item weights
   - Affect drag speed formula
   - Power-up mod (strength = 0 weight)

8. **CameraController.cs** - Quản lý camera (pan, zoom nếu cần)

9. **UIController.cs** - Quản lý UI trong gameplay
   - Update score, timer, weight
   - Show/update power-up inventory
   - Fade in/out panels

**Animations cần:**
- Miner swing animation
- Hook rotation animation
- Item collected animation (shine, popup)
- Power-up usage animation
- Win/Lose transition animation

**Audio cần:**
- dig.ogg - khi hook trúng item
- put_gem.ogg - khi item được kéo
- Collect-currency.ogg - khi collect item
- Gfx-Pull1.wav, Gfx-Pull2.wav - khi kéo
- Music-Level.ogg - bg music
- Value-Low/Normal/High.wav - khi lấy item khác giá trị
- Sfx-OhOh.wav - khi kalah

---

### 1.4 Shop Scene
**Assets cần:**
- Background image
- Shop items display (3D mock-up hoặc grid)
- Currency display (coins from previous levels)
- Buy buttons
- Back button

**Scripts cần:**
- `ShopManager.cs` (quản lý shop)
- `ShopItem.cs` (model item shop)

**Logic:**
- Display available power-ups:
  - Bomb (cost: 100 coins): xóa current item
  - Strength (cost: 150 coins): bỏ qua weight 10s
  - Time Boost (cost: 50 coins): +10s
- Click buy → add to inventory (save to PlayerPrefs)
- Back → return LevelSelect
- Show player total coins

**Audio**: SFX_Button.wav, SFX_Popup1.wav (buy action)

---

### 1.5 GameOver/Win Panel (Overlay)
**Assets cần:**
- Panel background
- Result text (Win/Lose)
- Score display
- Button: "Retry", "Next Level" (if win), "Main Menu"

**Scripts cần:**
- `EndLevelUI.cs`

**Logic:**
- Win: show "Level Complete!", score, button "Next Level"
  - Level 1 → Level 2
  - Level 2 → Level 3
  - Level 3 → MainMenu
- Lose: show "Time's Up!", score, button "Retry"
- Save score to PlayerPrefs

**Audio**: win-game-level-15.ogg (win), Sfx-OhOh.wav (lose)

---

## 2. MANAGERS (CROSS-SCENE)

### 2.1 GameStateManager.cs
- Quản lý state toàn game (menu, playing, paused, shop)
- DontDestroyOnLoad pattern
- Level progression tracking

### 2.2 AudioManager.cs
- Centralized audio control
- Background music management
- SFX playback
- Volume control (tùy chọn)

### 2.3 PlayerProgressManager.cs
- Save/Load level progress (PlayerPrefs)
- Track coins/currency
- Track power-up inventory
- High scores

### 2.4 CurrencyManager.cs
- Track coins earned per level
- Calculate total coins
- Deduct coins on purchase

---

## 3. COMPONENTS/UTILITIES

### 3.1 AnimationStrings.cs
- Define animation parameter names (hash)
- Use Animator.StringToHash() for optimization

### 3.2 SetBoolBehavior.cs & SetFloatBehavior.cs
- Animator state machine behaviors (đã có skeleton)
- Set animator parameters on state enter/exit

### 3.3 ObjectPool.cs (Optional)
- Pool items, power-ups cho reuse
- Improve performance

### 3.4 InputHandler.cs
- Centralize input (touch/mouse)
- Hook rotation
- Power-up activation

---

## 4. DETAILED WORKFLOW BY SCENE

### MainMenu Flow:
1. Load MainMenu scene
2. Show menu with Play, Shop buttons
3. AudioManager plays Music-Menu
4. Play button → DontDestroyOnLoad GameManager
5. → Load LevelSelect

### LevelSelect Flow:
1. Show 3 level buttons
2. Each button stores levelIndex
3. Click Level → Load Level scene with levelIndex
4. Back → Load MainMenu

### Level Gameplay Flow:
1. Initialize level (load UI, spawn items, reset score/timer)
2. HookController rotate continuously (-30° to +30° typical)
3. Player tap/click to cast hook (set retract timer)
4. Hook collides with item → ItemController.OnHook()
5. Calculate total weight → drag speed
6. If power-up active, modify speed/behavior
7. Item reach top (y > threshold) → collect (award points)
8. Timer counts down
9. Win condition: score ≥ target when time reaches 0 OR voluntarily end
10. Show EndLevelUI
11. Save coins to ProjectProgress
12. Click "Next Level" or "Retry"

### Shop Flow:
1. Display power-ups with prices
2. Show player coins (from previous levels)
3. Click buy → check coins, deduct, add to inventory
4. Inventory saved to PlayerPrefs
5. Back → Load LevelSelect

---

## 5. DATA STRUCTURES

### Item Data:
```
- Type: Enum (Gold, Stone, Diamond)
- Weight: float (1.0, 2.0, 3.0)
- Value: int (10, 5, 30)
- Speed multiplier: weight affects drag speed
```

### PowerUp Data:
```
- Type: Enum (Bomb, Strength, TimeBoost)
- Cost: int
- Duration: float (for Strength)
- Effect: function
```

### Level Data:
```
- LevelIndex: int
- ItemCount: int
- TargetScore: int
- Time: int (60s)
- ItemSpawnPositions: Vector2[]
```

---

## 6. PRIORITY CHECKLIST (Implementation Order)

### Phase 1: Core Gameplay Loop
- [ ] HookController (rotate, cast, retract)
- [ ] ItemController (attach to hook, drag)
- [ ] WeightCalculator (affect drag speed)
- [ ] ScoreManager (track points)
- [ ] TimerManager (60s countdown)
- [ ] LevelManager (win/lose logic)
- [ ] Basic Level Scene UI

### Phase 2: UI & Navigation
- [ ] MainMenu Scene + MenuManager
- [ ] LevelSelect Scene + LevelSelectManager
- [ ] GameOver/Win Panel + EndLevelUI
- [ ] UIController (update HUD)

### Phase 3: Power-ups
- [ ] PowerUpController (bomb, strength, time boost)
- [ ] Shop Scene + ShopManager
- [ ] Inventory system (PlayerPrefs)

### Phase 4: Audio & Polish
- [ ] AudioManager setup
- [ ] Add SFX to game events
- [ ] Add music to scenes
- [ ] Animations & visual feedback

### Phase 5: Persistence & Testing
- [ ] PlayerProgressManager
- [ ] Save/Load system
- [ ] Test all 3 level progression
- [ ] Bug fixes & optimization

---

## 7. ASSETS TO FIND/CREATE

### Sprites:
- Miner character
- Hook/lure
- Rope/chain
- Gold pile/gem
- Stone/rock
- Diamond
- Bomb icon
- Strength potion bottle
- Clock icon (time boost)

### Backgrounds:
- Mine/underground theme
- Menu background
- Shop background

### Audio (✓ = đã có):
- ✓ Music-Menu.ogg
- ✓ Music-Level.ogg
- ✓ dig.ogg
- ✓ put_gem.ogg
- ✓ Collect-currency.ogg
- ✓ SFX_Button.wav
- ✓ win-game-level-15.ogg
- ✓ Sfx-OhOh.wav
- (More found in AudioClip folder)

---

## 8. IMPLEMENTATION NOTES

### Important Game Feel:
- Hook should rotate smoothly
- Items should drag realistically (weight = speed)
- Collecting item = satisfying visual + sound
- Power-up usage = clear visual effect

### Performance:
- Use ObjectPool for frequently spawned items
- AnimatorStringToHash() for animation params
- Avoid instantiating items, use pool instead

### Extensibility:
- LevelData scriptable object for easy level design
- ItemData scriptable object for item configs
- PowerUpData scriptable object for shop items

---

## 9. SCRIPTS: PHÂN TÍCH DEPENDENCY

| **Script** | **Dependencies** | **Depends On** |
|--|--|--|
| HookController | Input | ItemController, WeightCalculator |
| ItemController | Rigidbody2D | HookController, WeightCalculator |
| ScoreManager | UIController | — |
| TimerManager | UIController | LevelManager |
| LevelManager | — | ScoreManager, TimerManager |
| PowerUpManager | — | HookController, TimerManager, ItemController |
| MainMenu | AudioManager | — |
| LevelSelectManager | GameStateManager | — |
| EndLevelUI | ScoreManager, CurrencyManager | — |
| ShopManager | CurrencyManager, PowerUpManager | — |
| UIController | — | ScoreManager, TimerManager, PowerUpManager |
| AudioManager | — | — |
| PlayerProgressManager | — | — |

**Best implementation order:**
1. HookController, ItemController (independent, core)
2. ScoreManager, TimerManager, LevelManager (depend on 1, but independent of each other)
3. PowerUpManager (depends on above)
4. MenuManager, LevelSelectManager (simple)
5. UIController (ties everything)
6. EndLevelUI, ShopManager (depends on systems)
7. AudioManager (applies to all)
8. PlayerProgressManager (final touches)

---

## 10. SCOPE BOUNDARIES

**Included:**
- 3 levels gameplay loop ✓
- Shop system (power-ups) ✓
- Weight mechanics ✓
- Timer + scoring ✓
- Audio + basic animations ✓

**NOT Included (for future):**
- Achievements / Leaderboard
- Additional effects (particles, screen shake)
- Online multiplayer
- Advanced AI obstacles
- Multiple difficulty modes

---

## 11. VERIFICATION CHECKLIST

### Phase 1 Test:
- [ ] Hook rotates smoothly
- [ ] Hook catches item on collision
- [ ] Drag speed changes with item weight
- [ ] Timer counts down 60s correctly
- [ ] Collect item → points awarded
- [ ] Timer = 0 → game ends

### Phase 2 Test:
- [ ] MainMenu → Play → LevelSelect works
- [ ] LevelSelect level buttons load correct level
- [ ] GameOver shows with correct score
- [ ] NextLevel flow: L1 → L2 → L3 → MainMenu ✓

### Phase 3 Test:
- [ ] Shop displays 3 power-ups
- [ ] Buy button deducts coins + adds to inventory
- [ ] Level loads power-ups from inventory
- [ ] Use bomb → removes attached item ✓
- [ ] Use strength → weight = 0 for 10s ✓
- [ ] Use time boost → +10s to timer ✓

### Phase 4 Test:
- [ ] Music plays on scene load
- [ ] SFX plays on events (collect, dig, etc.)
- [ ] Animations play smoothly (no jank)

### Phase 5 Test:
- [ ] Coins saved after level 1 → appears in shop
- [ ] Progress saved → can reload game and continue
- [ ] All 3 levels completable → full playthrough ✓

---

**Ngày tạo**: 1/4/2026  
**Status**: Ready for Phase 1 Implementation
