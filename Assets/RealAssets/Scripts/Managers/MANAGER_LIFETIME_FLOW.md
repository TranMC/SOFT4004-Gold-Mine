# Manager Lifetime Flow (RealAssets)

## 1) Tong quan lifecycle

### Manager ton tai xuyen scene (Awake + DontDestroyOnLoad)
- AudioManager
- GameManager
- InventoryManager
- LevelManager
- PlayerProgressManager

### Manager ton tai theo scene (tao moi moi lan vao scene)
- MainMenuController (MainMenu scene)
- ShopManager (Shop scene)
- ResultPanel (Level scene)
- UIManager (Level scene)

## 2) Flow tong the theo scene

1. MainMenu vao game:
- MainMenuController.OnStartButton()
- GameManager.OnPlayButtonClicked()
- LevelManager.StartNewRun()
- InventoryManager.StartNewRun()
- GameManager.StartLevel(1)

2. Vao Level:
- GameManager.StartLevel(levelIndex)
- LevelManager.InitializeLevel(levelIndex)
- LevelManager spawn prefab level, set target, timer, score, apply queued buff

3. Dang choi Level:
- ItemController.Collect() -> LevelManager.AddCollectedItemValue(value)
- LevelManager cong score + coin, cap nhat UI
- LevelManager.Update() dem timer
- Du target -> GameManager.HandleWin()
- Het gio -> GameManager.HandleLose()

4. Hien panel ket qua:
- ResultPanel bat event GameManager.OnGameStateChanged
- Win: ResultPanel.ShowPass() -> AutoProceedAfterPass()
- Lose: ResultPanel.ShowFail(), cho nguoi choi bam ve menu

5. Win flow:
- Save best score qua PlayerProgressManager
- Neu con level: LevelManager.AdvanceLevel() + GameManager.GoToShop()
- Neu het level: reset run + ve MainMenu

6. Shop flow:
- ShopManager.RefreshShopItems() moi lan vao shop
- ShopManager.BuyItem() tru coin, them item inventory, queue buff cho level tiep theo
- ShopManager.OnContinueButtonClicked() -> GameManager.GoToNextLevelFromShop()

7. Audio flow:
- AudioManager sceneLoaded -> PlayMusicForScene()
- Nut mute/unmute goi AudioManager.ToggleMute(), icon UI update qua event OnMuteStateChanged

## 3) Chi tiet tung manager

## AudioManager
Muc dich:
- Quan ly nhac nen theo scene
- Quan ly one-shot SFX
- Quan ly mute on/off xuyen scene va luu PlayerPrefs

Lifetime:
- Singleton + DontDestroyOnLoad

Bien:
- public static AudioManager Instance
- private const string MutePrefKey = "AudioManager_Muted"
- [SerializeField] AudioClip menuMusic
- [SerializeField] AudioClip levelMusic
- [SerializeField] AudioClip shopMusic
- [SerializeField] AudioSource musicSource
- [SerializeField] AudioSource sfxSource
- [SerializeField] bool playMusicOnSceneLoaded = true
- public bool IsMuted { get; private set; }
- public event System.Action<bool> OnMuteStateChanged

Ham va thoi diem dung:
- Awake(): khoi tao singleton, tao source neu thieu, load mute state
- OnEnable()/OnDisable(): dang ky/huy sceneLoaded
- Start(): phat nhac scene hien tai
- PlayMusic(AudioClip clip, bool loop = true): phat BGM
- StopMusic(): dung BGM
- PlaySfx(AudioClip clip, float volumeScale = 1f): phat SFX
- PlayMusicForScene(string sceneName): chon clip theo ten scene
- ToggleMute(): dao state mute
- SetMute(bool mute): set mute + luu PlayerPrefs + phat event

## GameManager
Muc dich:
- Dieu huong scene va state machine toan game
- Nhan ket qua win/lose va chuyen flow theo level/shop/menu

Lifetime:
- Singleton + DontDestroyOnLoad

Bien:
- public static GameManager Instance
- [SerializeField] string mainMenuScene = "MainMenu"
- [SerializeField] string levelScene = "Level"
- [SerializeField] string shopScene = "Shop"
- public GameState CurrentState { get; private set; } = GameState.Menu
- public event Action<GameState> OnGameStateChanged

Ham va thoi diem dung:
- Awake(): singleton + frame rate setup
- Start(): state ban dau = Menu
- SetState(GameState newState): doi state, pause/resume timer level, ban event
- GoToMainMenu(): ve main menu
- OnPlayButtonClicked(): reset run + vao level 1
- StartLevel(int levelIndex): set level hien tai, load scene level, init level
- PauseGame()/ResumeGame(): pause/resume game state
- HandleWin()/HandleLose(): set state ket qua
- GoToShop(): load shop scene
- GoToNextLevelFromShop(): neu con level thi qua level tiep, khong thi ve menu
- ReplayCurrentLevel(): choi lai level hien tai
- OnQuitButtonClicked(): thoat game/editor

## InventoryManager
Muc dich:
- Quan ly coin theo run
- Quan ly vat pham va so luong theo run bang Dictionary

Lifetime:
- Singleton + DontDestroyOnLoad

Bien:
- public static InventoryManager Instance
- private readonly Dictionary<ShopItemType, int> itemCounts
- [SerializeField] int runCoins = 0
- public int RunCoins => runCoins
- public event System.Action<int> OnRunCoinsChanged

Ham va thoi diem dung:
- Awake(): singleton + clamp coin >= 0
- StartNewRun(): reset coin va item theo run
- AddCoins(int amount): cong coin khi collect item
- TrySpendCoins(int amount): tru coin khi mua item shop
- AddItem(ShopItemType itemType, int amount = 1): them item
- HasItem(ShopItemType itemType): check item
- UseItem(ShopItemType itemType): dung item
- GetCount(ShopItemType itemType): lay so luong item
- AddItem/HasItem/UseItem/GetCount (string): API tuong thich script cu
- TryParseItemType(string itemId, out ShopItemType parsed): map string -> enum

## LevelManager
Muc dich:
- Quan ly current level, target, timer, score
- Spawn/xoa prefab level theo level
- Quan ly buff cho level tiep theo (strength, extra time, gold multiplier)

Lifetime:
- Singleton + DontDestroyOnLoad

Bien:
- public static LevelManager Instance
- [SerializeField] List<LevelDefinition> levels
- [SerializeField] Transform levelRoot
- [SerializeField] int currentLevel = 1
- const int MinLevel = 1
- const int MaxLevel = 3
- GameObject currentLevelInstance
- bool hasQueuedStrengthBoost
- int queuedExtraTimeSeconds
- float queuedGoldMultiplier = 1f
- int currentScore
- float remainingTime
- bool isTimerRunning
- public int CurrentLevel
- public int CurrentScore
- public float RemainingTime
- public int TargetScore { get; private set; } = 500
- public float CurrentLevelDuration { get; private set; } = 60f
- public bool StrengthBoostActiveThisLevel { get; private set; }
- public float GoldMultiplierThisLevel { get; private set; } = 1f
- public event System.Action<int> OnLevelChanged

Ham va thoi diem dung:
- Awake(): singleton + clamp level
- OnEnable()/OnDisable(): dang ky/huy sceneLoaded
- Update(): dem timer level, het gio -> lose
- InitializeLevel(int levelIndex): setup level full (data, prefab, buff, score, timer, UI)
- SetCurrentLevel(int level): set level hien tai
- HasNextLevel(): check con level tiep theo
- AdvanceLevel(): tang level len +1
- StartNewRun(): reset state run-level
- QueueStrengthBoostForNextLevel(): dat buff strength cho level tiep
- QueueExtraTimeForNextLevel(int seconds): dat them giay cho level tiep
- QueueGoldMultiplierForNextLevel(float multiplier): dat multiplier vang cho level tiep
- ApplyScoreModifiers(int baseScore): tinh diem sau buff
- AddCollectedItemValue(int baseValue): cong diem + coin + check win
- AddTime(float amount): cong them timer runtime
- PauseTimer()/ResumeTimer(): dung/tiep tuc timer
- ResetLevel(): xoa prefab level cu
- SpawnLevelPrefab(): spawn prefab theo level
- ApplyLevelData(int levelIndex): load target/time theo config
- FindDefinition(int levelIndex): tim config level
- EnsureLevelRoot(): dam bao co root spawn
- HandleSceneLoaded(Scene scene, LoadSceneMode mode): reset levelRoot khi vao scene level

## MainMenuController
Muc dich:
- Xu ly nut UI cua main menu (start/continue/settings/quit/sound)

Lifetime:
- Scene-bound (MainMenu)

Bien:
- [SerializeField] string gameplaySceneName = "GameScene" (hien tai khong con su dung truc tiep de load)
- [SerializeField] GameObject settingsPanel
- const string LastUnlockedLevelKey = "LastUnlockedLevel"
- const string HasSaveKey = "HasSaveData"
- [SerializeField] Image soundIcon
- [SerializeField] Sprite soundOnSprite
- [SerializeField] Sprite soundOffSprite

Ham va thoi diem dung:
- Start(): an panel settings ban dau
- OnStartButton(): danh dau save key + goi GameManager play
- OnContinueButton(): neu co save thi vao level hien tai
- OnSettingsButton()/OnCloseSettingsButton(): mo/dong panel settings
- OnQuitButton(): thoat game
- OnEnable()/OnDisable(): dang ky/huy event mute
- OnSoundToggleClicked(): bat/tat am thanh
- HandleMuteStateChanged(bool isMuted): doi sprite icon sound

## PlayerProgressManager
Muc dich:
- Luu best score tung level qua cac run

Lifetime:
- Singleton + DontDestroyOnLoad

Bien:
- public static PlayerProgressManager Instance
- private const string BestScorePrefix = "PPM_BestScore_Level_"

Ham va thoi diem dung:
- Awake(): singleton
- SaveBestScore(int level, int score): luu neu score moi cao hon
- GetBestScore(int level): doc best score
- ResetBestScores(): xoa key best score (1..10)
- GetBestScoreKey(int level): tao key PlayerPrefs

## ResultPanel
Muc dich:
- Hien pass/fail panel va dieu huong sau ket qua level

Lifetime:
- Scene-bound (Level)

Bien:
- [SerializeField] GameObject panel
- [SerializeField] TMP_Text messageText
- [SerializeField] Button backToMenuButton
- [SerializeField] float showDelay = 1f
- [SerializeField] string passMessage
- [SerializeField] string failMessage

Ham va thoi diem dung:
- Awake(): hide panel, bind button
- OnEnable()/OnDisable(): subscribe GameManager.OnGameStateChanged
- HandleStateChanged(GameState state): state -> show pass/fail
- ShowWithDelay(bool isPass): delay roi hien panel
- ShowPass(): hien panel pass + auto proceed
- ShowFail(): hien panel fail + nut back
- AutoProceedAfterPass(): save best score, qua shop hoac reset run ve menu
- OnBackToMenuClicked(): reset run va ve menu

## ShopManager
Muc dich:
- Quan ly shop trong scene Shop:
- Refresh item moi moi lan vao
- Mua item, tru coin, them item inventory, queue buff level tiep

Lifetime:
- Scene-bound (Shop)

Bien:
- public static ShopManager Instance
- [SerializeField] TextMeshProUGUI coinText
- [SerializeField] TextMeshProUGUI descriptionText
- [SerializeField] ShopItemUI itemPrefab
- [SerializeField] Transform itemContainer
- [SerializeField] List<ShopItem> itemCatalog
- [SerializeField] int itemCountPerVisit = 3
- private readonly List<ShopItemUI> generatedItems

Ham va thoi diem dung:
- Awake(): singleton scene-level
- Start(): refresh shop + cap nhat coin + subscribe coin event
- OnDestroy(): unsubscribe coin event
- ShowDescription(string desc)/HideDescription(): update text mo ta hover
- BuyItem(ShopItemUI itemUI, ShopItem itemData): mua item va apply queue buff
- UpdateCoinUI(int coin)/UpdateCoinUI(): cap nhat text coin
- OnContinueButtonClicked(): qua level tiep theo
- RefreshShopItems(): xoa item cu va generate item moi tu catalog

## UIManager
Muc dich:
- Quan ly UI gameplay HUD va panel pause/win/lose

Lifetime:
- Scene-bound (Level)

Bien:
- public static UIManager Instance
- [SerializeField] TMP_Text scoreText
- [SerializeField] TMP_Text timerText
- [SerializeField] TMP_Text targetText
- [SerializeField] TMP_Text capText
- [SerializeField] GameObject pausePanel
- [SerializeField] GameObject winPanel
- [SerializeField] GameObject losePanel

Ham va thoi diem dung:
- Awake(): singleton scene-level
- OnEnable()/OnDisable(): subscribe GameManager.OnGameStateChanged
- UpdateScoreUI(int score): cap nhat score HUD
- UpdateTimeUI(float timeRemaining): cap nhat timer HUD
- UpdateTargetUI(int target): cap nhat target HUD
- UpdateCapUI(int level): cap nhat level HUD
- UpdateInventoryUI(): placeholder de tuong thich call cu
- HandleGameStateChanged(GameState state): bat/tat panel theo state
- SetPanelActive(GameObject panel, bool active): helper set active

## 4) Mapping nhanh: manager nao quan ly cai gi

- Scene flow va game state: GameManager
- Level flow/runtime (level index, target, timer, score, prefab level, queued buff): LevelManager
- Run economy (coin + item dictionary): InventoryManager
- Shop runtime (refresh item, buy, queue buff): ShopManager
- Audio cross-scene + mute toggle state: AudioManager
- Save best score qua run: PlayerProgressManager
- Menu buttons: MainMenuController
- Result panel dieu huong sau win/lose: ResultPanel
- Gameplay HUD panels: UIManager
