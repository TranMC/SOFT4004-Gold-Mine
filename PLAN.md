# Ke Hoach Chi Tiet: Gold Mine Game - Thu Thap Vang 2D

## 1. Thong Tin Da Chot
- Scope: 3 levels tuyen tinh, khong co Level Select
- Thoi gian moi level: 60 giay
- Item gameplay: Vang, Da, Kim Cuong (co trong luong rieng)
- Power-ups: Bom, Strength, Time Boost (+10s)
- MainMenu: chi co nut Play
- Pass/Fail message:
  - Pass: "ban da len cap do tiep"
  - Fail: "ban chua hoan thanh nhiem vu"
- Sau khi pass Level 3: ve MainMenu luon (khong vao Shop)

---

## 2. Scene Architecture
Can 3 scene chinh:
1. MainMenu
2. Level (dung chung 1 scene gameplay, load prefab theo currentLevel)
3. Shop

Khong dung scene LevelSelect.

---

## 3. Flow Tong The (Dung theo yeu cau)
1. Nguoi choi mo game -> vao MainMenu
2. Bam Play -> set currentLevel = 1 -> load scene Level voi prefab Level 1
3. Choi het 60s:
   - Neu dat targetScore: hien "ban da len cap do tiep"
   - Neu khong dat targetScore: hien "ban chua hoan thanh nhiem vu" -> quay ve MainMenu
4. Neu pass Level 1 hoac Level 2:
   - Chuyen vao Shop
   - Nguoi choi mua hoac khong mua
   - Bam "Choi tiep" -> currentLevel++ -> quay lai scene Level va load prefab level tiep theo
5. Neu pass Level 3:
   - Hien "ban da len cap do tiep"
   - Quay ve MainMenu (ket thuc vong 3 level)

---

## 4. Chi Tiet Tung Scene

### 4.1 MainMenu Scene
Assets can:
- Background menu
- Logo game
- Nut Play
- Nhac nen menu

Scripts can:
- `MenuManager.cs`

Attributes de xai trong script:
- `string levelSceneName`
- `AudioClip menuMusic`

Logic:
- `OnPlayClicked()`:
  - `GameStateManager.currentLevel = 1`
  - Reset state tam thoi cua run hien tai (neu can)
  - Load `Level` scene

Animation/SFX:
- Fade in UI
- SFX click nut

---

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

### 4.3 Shop Scene
Assets can:
- Background shop
- 3 item cards power-up
- Price text
- Coin text
- Nut "Choi tiep"

Scripts can:
- `ShopManager.cs`
- `ShopItem.cs`

#### ShopManager.cs
Attributes:
- `int playerCoins`
- `int bombPrice = 100`
- `int strengthPrice = 150`
- `int timeBoostPrice = 50`
- `Button continueButton`

Logic:
- Hien coin hien tai
- Buy item neu du coin
- Luu inventory vao `PlayerPrefs`
- `OnContinueClicked()`:
  - `GameStateManager.currentLevel++`
  - Load scene Level

Luu y:
- Shop chi xuat hien sau khi pass Level 1 va Level 2
- Pass Level 3 khong vao Shop

Animation/SFX:
- Popup buy, button click

---

## 5. Managers Xuyen Scene

### 5.1 GameStateManager.cs
Trach nhiem:
- Giu `currentLevel` (1..3)
- Giu coin tam run hien tai
- Dieu huong flow pass/fail
- DontDestroyOnLoad

Attributes goi y:
- `int currentLevel`
- `int totalCoins`

### 5.2 AudioManager.cs
Trach nhiem:
- Play/Stop music theo scene
- Play one-shot SFX

### 5.3 PlayerProgressManager.cs
Trach nhiem:
- Save/Load coins
- Save inventory power-up
- Save best score tung level (neu can)

---

## 6. Data Design

### Item Data
- Type: Gold / Stone / Diamond
- Value: tuy chinh
- Weight: tuy chinh

### Level Data
- LevelIndex: 1, 2, 3
- TargetScore
- Duration: 60
- PrefabRef

### Shop Data
- ItemType: Bomb / Strength / TimeBoost
- Price
- Quantity owned

---

## 7. Priority Todo (Implementation Order)

### Phase 1: Core Gameplay
- [ ] HookController
- [ ] ItemController
- [ ] WeightCalculator
- [ ] ScoreManager
- [ ] TimerManager
- [ ] LevelManager + load prefab theo currentLevel
- [ ] UI co Score/Timer/Target

### Phase 2: Flow dung yeu cau
- [ ] MainMenu chi co nut Play
- [ ] Pass/Fail panel voi 2 message da chot
- [ ] Fail -> quay ve MainMenu
- [ ] Pass Level 1/2 -> vao Shop
- [ ] Pass Level 3 -> ve MainMenu

### Phase 3: Shop + Power-ups
- [ ] ShopManager + ShopItem
- [ ] Buy/skip item
- [ ] Nut Choi tiep -> level ke
- [ ] Power-up inventory apply vao Level

### Phase 4: Audio + Animation + Polish
- [ ] Music menu/level/shop
- [ ] SFX gameplay va UI
- [ ] Fade panel, feedback collect

### Phase 5: Save/Load
- [ ] PlayerPrefs for coins + inventory
- [ ] Basic reset/test hooks

---

## 8. Verification Checklist

### Main Flow Test
- [ ] MainMenu bam Play vao thang Level 1
- [ ] Fail Level 1: hien "ban chua hoan thanh nhiem vu" va ve MainMenu
- [ ] Pass Level 1: hien "ban da len cap do tiep" va vao Shop
- [ ] Tu Shop bam Choi tiep vao Level 2
- [ ] Pass Level 2: vao Shop roi Choi tiep vao Level 3
- [ ] Pass Level 3: ve MainMenu (khong vao Shop)

### Gameplay Test
- [ ] Hook quay + phong + keo dung
- [ ] Trong luong anh huong toc do keo
- [ ] Score va timer cap nhat dung
- [ ] Time Boost cong 10s dung
- [ ] Bom xoa item dang keo dung
- [ ] Strength bo qua weight trong thoi gian hieu luc

### Shop Test
- [ ] Mua item khi du coin
- [ ] Khong mua duoc khi thieu coin
- [ ] Co the bo qua mua va bam Choi tiep
- [ ] Inventory duoc mang sang level ke

---

## 9. Asset Checklist Can Tim

Sprites:
- Miner
- Hook
- Rope/chain
- Gold, Stone, Diamond
- Bomb icon
- Strength potion icon
- Time boost icon

Background:
- MainMenu
- Level mine (it nhat 3 variation)
- Shop

Audio:
- Menu music
- Level music
- Shop ambience/music
- UI click
- Hook launch/pull
- Item collect
- Pass/fail stinger

---

## 10. Notes Trien Khai
- Nen dung 1 gameplay scene + 3 level prefabs de de bao tri.
- Tach `GameStateManager` ro rang de khong loi flow scene.
- Uu tien dung flow pass/fail truoc roi moi polish.

---

**Ngay cap nhat**: 1/4/2026
**Trang thai**: Plan da viet lai moi, dung flow 3 level tuyen tinh theo yeu cau.
