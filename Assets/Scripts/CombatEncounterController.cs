using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatEncounterController : MonoBehaviour
{
    [Serializable]
    private class AttackDefinition
    {
        public string attackName = "Slash";
        public int minDamage = 6;
        public int maxDamage = 14;
    }

    [Serializable]
    private class EnemyUnit
    {
        public string unitName = "Card Soldier";
        public int maxHealth = 30;
        public GameObject visualRoot;
        public Collider2D targetCollider;
        [NonSerialized] public int currentHealth;

        public bool IsAlive => currentHealth > 0;
    }

    private class DamagePopup
    {
        public bool isPlayerTarget;
        public EnemyUnit target;
        public int amount;
        public float timeRemaining;
    }

    [Header("Encounter")]
    [SerializeField] private bool startCombatOnPlay = true;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera combatCamera;
    [SerializeField] private GameObject playerVisualRoot;

    [Header("Attack Teleport")]
    [SerializeField] private Transform playerAttackTeleportSpotSoldierA;
    [SerializeField] private Transform playerAttackTeleportSpotSoldierB;
    [SerializeField] private Transform playerAttackTeleportSpotQueen;
    [SerializeField] private Transform enemyAttackTeleportSpot;
    [SerializeField] private float preAttackTeleportHold = 0.2f;
    [SerializeField] private float postAttackReturnDelay = 0.05f;

    [Header("Audio")]
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioSource enemyAttackAudioSourceA;
    [SerializeField] private AudioSource enemyAttackAudioSourceB;
    [SerializeField] private AudioClip teleportSfx;
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private float teleportSfxVolume = 0.9f;
    [SerializeField] private float attackSfxVolume = 0.55f;
    [SerializeField] private float teleportSfxStartTime = 0.03f;

    [Header("Player")]
    [SerializeField] private int playerMaxHealth = 100;
    [SerializeField] private AttackDefinition[] attacks =
    {
        new AttackDefinition { attackName = "Quick Jab", minDamage = 4, maxDamage = 9 },
        new AttackDefinition { attackName = "Heart Cutter", minDamage = 8, maxDamage = 15 },
        new AttackDefinition { attackName = "Royal Break", minDamage = 12, maxDamage = 22 }
    };

    [Header("Enemy")]
    [SerializeField] private EnemyUnit cardSoldierA = new EnemyUnit { unitName = "Card Soldier A", maxHealth = 30 };
    [SerializeField] private EnemyUnit cardSoldierB = new EnemyUnit { unitName = "Card Soldier B", maxHealth = 30 };
    [SerializeField] private EnemyUnit redQueen = new EnemyUnit { unitName = "Red Queen", maxHealth = 120 };

    [Header("Reaction Timing")]
    [SerializeField] private Vector2 goSignalDelayRange = new Vector2(0.6f, 1.6f);
    [SerializeField] private float perfectClickTimeAfterGo = 0.35f;
    [SerializeField] private float fullMissWindow = 0.8f;

    [Header("Enemy Turn")]
    [SerializeField] private Vector2Int enemyDamageRange = new Vector2Int(5, 15);

    [Header("Queen Phase")]
    [SerializeField] private Vector2 queenChargeTimeRange = new Vector2(0.8f, 2.0f);
    [SerializeField] private float queenDodgeWindowDuration = 0.7f;
    [SerializeField] private Vector2Int queenAttackDamageRange = new Vector2Int(10, 20);
    [SerializeField] private Vector2Int queenCounterDamageRange = new Vector2Int(15, 28);
    [SerializeField] private float queenDodgeBarWidth = 220f;
    [SerializeField] private float queenDodgeBarHeight = 14f;

    [Header("UI - Damage Feedback")]
    [SerializeField] private Texture2D damageIcon;
    [SerializeField] private float damagePopupLifetime = 0.85f;
    [SerializeField] private float damageIconScaleOnTarget = 0.95f;
    [SerializeField] private float damageIconVerticalOffset = 24f;
    [SerializeField] private float damageNumberYOffset = 16f;
    [SerializeField] private Color damageTextColor = new Color(0.95f, 0.2f, 0.2f);

    [Header("UI - Health Bars")]
    [SerializeField] private float playerHealthBarWidth = 110f;
    [SerializeField] private float enemyHealthBarWidth = 110f;
    [SerializeField] private float enemyHealthBarHeight = 9f;
    [SerializeField] private Color healthFillColor = new Color(0.3f, 0.85f, 0.35f);
    [SerializeField] private Color healthBackColor = new Color(0.16f, 0.16f, 0.16f);

    [Header("Enemy Highlight")]
    [SerializeField] private Color hoverTint = new Color(1f, 0.95f, 0.75f, 1f);
    [SerializeField] private Color selectedTint = new Color(0.8f, 1f, 0.85f, 1f);

    private enum CombatState
    {
        Inactive,
        ChooseAttack,
        WaitingForGo,
        WaitForClick,
        EnemyTurn,
        Animating,
        QueenCharging,
        QueenDodgeWindow,
        Victory,
        Defeat
    }

    private CombatState currentState = CombatState.Inactive;
    private int playerHealth;
    private int selectedAttackIndex = -1;
    private EnemyUnit selectedTarget;
    private EnemyUnit hoveredTarget;
    private float stateTimer;
    private string battleMessage = string.Empty;

    private float queenChargeTotal;

    private readonly List<DamagePopup> damagePopups = new List<DamagePopup>();
    private readonly Dictionary<EnemyUnit, Rect> enemyScreenRects = new Dictionary<EnemyUnit, Rect>();
    private readonly Dictionary<SpriteRenderer, Color> originalSpriteColors = new Dictionary<SpriteRenderer, Color>();

    private GUIStyle damageNumberStyle;
    private Texture2D solidTexture;

    private bool PlayerWon => !redQueen.IsAlive;

    private void Start()
    {
        if (playerAudioSource == null)
        {
            playerAudioSource = GetComponent<AudioSource>();
        }

        if (enemyAttackAudioSourceA == null)
        {
            enemyAttackAudioSourceA = playerAudioSource;
        }

        if (enemyAttackAudioSourceB == null)
        {
            enemyAttackAudioSourceB = playerAudioSource;
        }

        ConfigureAudioSourceForSfx(playerAudioSource);
        ConfigureAudioSourceForSfx(enemyAttackAudioSourceA);
        ConfigureAudioSourceForSfx(enemyAttackAudioSourceB);

        if (teleportSfx != null)
        {
            teleportSfx.LoadAudioData();
        }

        if (attackSfx != null)
        {
            attackSfx.LoadAudioData();
        }

        InitializeEncounter();

        if (startCombatOnPlay)
        {
            BeginCombat();
        }
    }

    private void InitializeEncounter()
    {
        playerHealth = Mathf.Max(1, playerMaxHealth);
        cardSoldierA.currentHealth = Mathf.Max(1, cardSoldierA.maxHealth);
        cardSoldierB.currentHealth = Mathf.Max(1, cardSoldierB.maxHealth);
        redQueen.currentHealth = Mathf.Max(1, redQueen.maxHealth);

        selectedAttackIndex = -1;
        selectedTarget = null;
        hoveredTarget = null;
        stateTimer = 0f;
        battleMessage = string.Empty;
        damagePopups.Clear();

        currentState = CombatState.Inactive;

        SetUnitVisible(cardSoldierA, true);
        SetUnitVisible(cardSoldierB, true);
        SetUnitVisible(redQueen, true);

        SetPlayerMovementLocked(false);
        RefreshAllUnitHighlights();
    }

    public void BeginCombat()
    {
        if (currentState == CombatState.Victory || currentState == CombatState.Defeat)
        {
            InitializeEncounter();
        }

        selectedAttackIndex = -1;
        selectedTarget = null;
        hoveredTarget = null;
        battleMessage = "Choose attack, then click a soldier.";
        currentState = CombatState.ChooseAttack;
        SetPlayerMovementLocked(true);
        RefreshAllUnitHighlights();
    }

    private void Update()
    {
        UpdateDamagePopups();
        UpdateEnemyMouseSelection();

        if (currentState == CombatState.WaitingForGo)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                stateTimer = 0f;
                currentState = CombatState.WaitForClick;
                battleMessage = "GO! Click now.";
            }
            return;
        }

        if (currentState == CombatState.WaitForClick)
        {
            stateTimer += Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                ResolvePlayerAttack();
                return;
            }

            if (stateTimer >= fullMissWindow)
            {
                battleMessage = "Too slow. You missed.";
                currentState = CombatState.EnemyTurn;
                stateTimer = 0.9f;
                selectedAttackIndex = -1;
                selectedTarget = null;
                RefreshAllUnitHighlights();
            }
            return;
        }

        if (currentState == CombatState.EnemyTurn)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                ResolveEnemyTurn();
            }
            return;
        }

        if (currentState == CombatState.QueenCharging)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                stateTimer = 0f;
                currentState = CombatState.QueenDodgeWindow;
                battleMessage = "DODGE! Press Space or click!";
            }
            return;
        }

        if (currentState == CombatState.QueenDodgeWindow)
        {
            stateTimer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.RightArrow) || Input.GetMouseButtonDown(0))
            {
                ResolveQueenDodge(true);
                return;
            }

            if (stateTimer >= queenDodgeWindowDuration)
            {
                ResolveQueenDodge(false);
            }
            return;
        }

        if (currentState == CombatState.Victory || currentState == CombatState.Defeat)
        {
            SetPlayerMovementLocked(false);
        }
    }

    private void StartPlayerAttack(int attackIndex)
    {
        if (currentState != CombatState.ChooseAttack)
        {
            return;
        }

        if (attackIndex < 0 || attackIndex >= attacks.Length)
        {
            return;
        }

        selectedAttackIndex = attackIndex;

        if (selectedTarget == null)
        {
            battleMessage = "Attack selected. Hover and click a target.";
            return;
        }

        BeginTimingPhase();
    }

    private void SelectTarget(EnemyUnit target)
    {
        if (currentState != CombatState.ChooseAttack)
        {
            return;
        }

        if (target == null || !target.IsAlive)
        {
            return;
        }

        if (target == redQueen && (cardSoldierA.IsAlive || cardSoldierB.IsAlive))
        {
            battleMessage = "Defeat both card soldiers first.";
            return;
        }

        selectedTarget = target;
        RefreshAllUnitHighlights();

        if (selectedAttackIndex < 0)
        {
            battleMessage = "Target selected. Pick an attack.";
            return;
        }

        BeginTimingPhase();
    }

    private void BeginTimingPhase()
    {
        currentState = CombatState.WaitingForGo;
        stateTimer = UnityEngine.Random.Range(goSignalDelayRange.x, goSignalDelayRange.y);
        battleMessage = "Ready... wait for GO.";
    }

    private void ResolvePlayerAttack()
    {
        if (selectedAttackIndex < 0 || selectedAttackIndex >= attacks.Length)
        {
            return;
        }

        if (selectedTarget == null || !selectedTarget.IsAlive)
        {
            currentState = CombatState.ChooseAttack;
            battleMessage = "Pick a living target.";
            return;
        }

        AttackDefinition attack = attacks[selectedAttackIndex];

        float distanceFromPerfect = Mathf.Abs(stateTimer - perfectClickTimeAfterGo);
        float accuracy = Mathf.Clamp01(1f - (distanceFromPerfect / fullMissWindow));
        int intendedDamage = Mathf.RoundToInt(Mathf.Lerp(attack.minDamage, attack.maxDamage, accuracy));
        int accuracyPercent = Mathf.RoundToInt(accuracy * 100f);

        EnemyUnit attackTarget = selectedTarget;
        currentState = CombatState.Animating;
        StartCoroutine(ExecutePlayerAttackSequence(attack, attackTarget, intendedDamage, accuracyPercent));
    }

    private int ApplyDamageToEnemy(EnemyUnit target, int damage)
    {
        int clampedDamage = Mathf.Max(0, damage);
        int previousHealth = target.currentHealth;
        target.currentHealth = Mathf.Max(0, target.currentHealth - clampedDamage);
        int dealtDamage = previousHealth - target.currentHealth;

        if (dealtDamage > 0)
        {
            SpawnDamagePopup(target, dealtDamage);
        }

        if (!target.IsAlive)
        {
            SetUnitVisible(target, false);
        }

        return dealtDamage;
    }

    private void ResolveEnemyTurn()
    {
        EnemyUnit attackingEnemy = GetRandomAliveCounterAttacker();
        if (attackingEnemy == null)
        {
            currentState = CombatState.ChooseAttack;
            battleMessage = "No enemies left to counterattack.";
            return;
        }

        int damage = UnityEngine.Random.Range(enemyDamageRange.x, enemyDamageRange.y + 1);
        currentState = CombatState.Animating;
        StartCoroutine(ExecuteEnemyAttackSequence(attackingEnemy, damage));
    }

    private System.Collections.IEnumerator ExecutePlayerAttackSequence(AttackDefinition attack, EnemyUnit target, int intendedDamage, int accuracyPercent)
    {
        GameObject playerRoot = GetPlayerVisualRootObject();
        Vector3 originalPlayerPosition = Vector3.zero;
        Transform attackSpot = GetPlayerAttackSpotForTarget(target);

        if (playerRoot != null)
        {
            originalPlayerPosition = playerRoot.transform.position;
            if (attackSpot != null)
            {
                PlaySfx(playerAudioSource, teleportSfx, teleportSfxVolume);
                playerRoot.transform.position = GetTeleportPositionKeepingDepth(attackSpot.position, originalPlayerPosition.z);
            }
        }

        battleMessage = attack.attackName + "!";
        yield return new WaitForSeconds(preAttackTeleportHold);

        if (target == null || !target.IsAlive)
        {
            if (playerRoot != null)
            {
                playerRoot.transform.position = originalPlayerPosition;
            }

            currentState = CombatState.ChooseAttack;
            battleMessage = "Target is gone. Choose attack and target.";
            yield break;
        }

        PlaySfx(playerAudioSource, attackSfx, attackSfxVolume);
        int dealtDamage = ApplyDamageToEnemy(target, intendedDamage);
        battleMessage = attack.attackName + " hit " + target.unitName + " for " + dealtDamage + " (" + accuracyPercent + "%).";

        if (playerRoot != null)
        {
            playerRoot.transform.position = originalPlayerPosition;
        }

        yield return new WaitForSeconds(postAttackReturnDelay);

        if (PlayerWon)
        {
            currentState = CombatState.Victory;
            battleMessage = "Red Queen defeated.";
            yield break;
        }

        selectedAttackIndex = -1;
        selectedTarget = null;
        hoveredTarget = null;
        RefreshAllUnitHighlights();

        if (!cardSoldierA.IsAlive && !cardSoldierB.IsAlive && redQueen.IsAlive)
        {
            BeginQueenPhase();
            yield break;
        }

        currentState = CombatState.EnemyTurn;
        stateTimer = 1.0f;
    }

    private void BeginQueenPhase()
    {
        battleMessage = "The Red Queen steps forward!";
        EnterQueenCharging();
    }

    private void EnterQueenCharging()
    {
        queenChargeTotal = UnityEngine.Random.Range(queenChargeTimeRange.x, queenChargeTimeRange.y);
        stateTimer = queenChargeTotal;
        currentState = CombatState.QueenCharging;
    }

    private void ResolveQueenDodge(bool playerDodged)
    {
        if (playerDodged)
        {
            float dodgeSpeed = Mathf.Clamp01(1f - (stateTimer / Mathf.Max(0.01f, queenDodgeWindowDuration)));
            int counterDamage = Mathf.RoundToInt(Mathf.Lerp(queenCounterDamageRange.x, queenCounterDamageRange.y, dodgeSpeed));
            currentState = CombatState.Animating;
            StartCoroutine(ExecuteQueenDodgeCounterSequence(counterDamage));
        }
        else
        {
            int damage = UnityEngine.Random.Range(queenAttackDamageRange.x, queenAttackDamageRange.y + 1);
            currentState = CombatState.Animating;
            StartCoroutine(ExecuteQueenHitSequence(damage));
        }
    }

    private System.Collections.IEnumerator ExecuteQueenDodgeCounterSequence(int counterDamage)
    {
        GameObject playerRoot = GetPlayerVisualRootObject();
        Vector3 originalPlayerPos = Vector3.zero;
        Transform attackSpot = playerAttackTeleportSpotQueen != null ? playerAttackTeleportSpotQueen : playerAttackTeleportSpotSoldierA;

        if (playerRoot != null && attackSpot != null)
        {
            originalPlayerPos = playerRoot.transform.position;
            PlaySfx(playerAudioSource, teleportSfx, teleportSfxVolume);
            playerRoot.transform.position = GetTeleportPositionKeepingDepth(attackSpot.position, originalPlayerPos.z);
        }

        battleMessage = "Dodged! Counter-attack!";
        yield return new WaitForSeconds(preAttackTeleportHold);

        PlaySfx(playerAudioSource, attackSfx, attackSfxVolume);
        int dealt = ApplyDamageToEnemy(redQueen, counterDamage);
        battleMessage = "Counter hit the Red Queen for " + dealt + "!";

        if (playerRoot != null && attackSpot != null)
        {
            playerRoot.transform.position = originalPlayerPos;
        }

        yield return new WaitForSeconds(postAttackReturnDelay);

        if (PlayerWon)
        {
            currentState = CombatState.Victory;
            battleMessage = "Red Queen defeated!";
            yield break;
        }

        EnterQueenCharging();
    }

    private System.Collections.IEnumerator ExecuteQueenHitSequence(int damage)
    {
        Vector3 originalQueenPos = Vector3.zero;
        bool moved = false;

        if (redQueen.visualRoot != null && enemyAttackTeleportSpot != null)
        {
            originalQueenPos = redQueen.visualRoot.transform.position;
            moved = true;
            PlaySfx(playerAudioSource, teleportSfx, teleportSfxVolume);
            redQueen.visualRoot.transform.position = GetTeleportPositionKeepingDepth(enemyAttackTeleportSpot.position, originalQueenPos.z);
        }

        battleMessage = "Too slow! The Red Queen strikes!";
        yield return new WaitForSeconds(preAttackTeleportHold);

        PlaySfx(playerAudioSource, attackSfx, attackSfxVolume);
        playerHealth = Mathf.Max(0, playerHealth - damage);
        SpawnPlayerDamagePopup(damage);

        if (moved)
        {
            redQueen.visualRoot.transform.position = originalQueenPos;
        }

        yield return new WaitForSeconds(postAttackReturnDelay);

        if (playerHealth <= 0)
        {
            currentState = CombatState.Defeat;
            battleMessage = "The Red Queen defeated you.";
            yield break;
        }

        battleMessage = "The Red Queen hit for " + damage + ". Brace yourself...";
        EnterQueenCharging();
    }

    private System.Collections.IEnumerator ExecuteEnemyAttackSequence(EnemyUnit attackingEnemy, int damage)
    {
        Vector3 originalEnemyPosition = Vector3.zero;
        bool movedEnemy = false;

        if (attackingEnemy != null && attackingEnemy.visualRoot != null)
        {
            originalEnemyPosition = attackingEnemy.visualRoot.transform.position;
            movedEnemy = true;

            if (enemyAttackTeleportSpot != null)
            {
                PlaySfx(GetEnemyAttackAudioSource(attackingEnemy), teleportSfx, teleportSfxVolume);
                attackingEnemy.visualRoot.transform.position = GetTeleportPositionKeepingDepth(enemyAttackTeleportSpot.position, originalEnemyPosition.z);
            }
        }

        battleMessage = attackingEnemy.unitName + " attacks!";
        yield return new WaitForSeconds(preAttackTeleportHold);

        PlaySfx(GetEnemyAttackAudioSource(attackingEnemy), attackSfx, attackSfxVolume);
        playerHealth = Mathf.Max(0, playerHealth - damage);
        SpawnPlayerDamagePopup(damage);

        if (movedEnemy)
        {
            attackingEnemy.visualRoot.transform.position = originalEnemyPosition;
        }

        yield return new WaitForSeconds(postAttackReturnDelay);

        if (playerHealth <= 0)
        {
            currentState = CombatState.Defeat;
            battleMessage = attackingEnemy.unitName + " hit for " + damage + ". You were defeated.";
            yield break;
        }

        selectedAttackIndex = -1;
        selectedTarget = null;
        hoveredTarget = null;
        RefreshAllUnitHighlights();
        currentState = CombatState.ChooseAttack;
        battleMessage = attackingEnemy.unitName + " hit for " + damage + ". Choose attack and target.";
    }

    private Transform GetPlayerAttackSpotForTarget(EnemyUnit target)
    {
        if (target == cardSoldierA)
        {
            return playerAttackTeleportSpotSoldierA;
        }

        if (target == cardSoldierB)
        {
            return playerAttackTeleportSpotSoldierB;
        }

        if (target == redQueen)
        {
            if (playerAttackTeleportSpotQueen != null)
            {
                return playerAttackTeleportSpotQueen;
            }

            if (playerAttackTeleportSpotSoldierB != null)
            {
                return playerAttackTeleportSpotSoldierB;
            }

            return playerAttackTeleportSpotSoldierA;
        }

        return null;
    }

    private static Vector3 GetTeleportPositionKeepingDepth(Vector3 targetPosition, float originalZ)
    {
        return new Vector3(targetPosition.x, targetPosition.y, originalZ);
    }

    private AudioSource GetEnemyAttackAudioSource(EnemyUnit attacker)
    {
        if (attacker == cardSoldierB)
        {
            return enemyAttackAudioSourceB != null ? enemyAttackAudioSourceB : playerAudioSource;
        }

        return enemyAttackAudioSourceA != null ? enemyAttackAudioSourceA : playerAudioSource;
    }

    private void PlaySfx(AudioSource source, AudioClip clip, float volumeScale)
    {
        if (clip == null || source == null)
        {
            return;
        }

        source.Stop();
        source.clip = clip;
        source.volume = Mathf.Clamp01(volumeScale);

        float startTime = 0f;
        if (clip == teleportSfx)
        {
            startTime = teleportSfxStartTime;
        }

        if (clip.length > 0.01f)
        {
            source.time = Mathf.Clamp(startTime, 0f, clip.length - 0.01f);
        }
        else
        {
            source.time = 0f;
        }

        source.Play();
    }

    private static void ConfigureAudioSourceForSfx(AudioSource source)
    {
        if (source == null)
        {
            return;
        }

        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.dopplerLevel = 0f;
    }

    private EnemyUnit GetRandomAliveCounterAttacker()
    {
        List<EnemyUnit> attackers = new List<EnemyUnit>(3);

        if (cardSoldierA.IsAlive)
        {
            attackers.Add(cardSoldierA);
        }

        if (cardSoldierB.IsAlive)
        {
            attackers.Add(cardSoldierB);
        }

        if (attackers.Count == 0 && redQueen.IsAlive)
        {
            attackers.Add(redQueen);
        }

        if (attackers.Count == 0)
        {
            return null;
        }

        int index = UnityEngine.Random.Range(0, attackers.Count);
        return attackers[index];
    }

    private void SpawnDamagePopup(EnemyUnit target, int damage)
    {
        damagePopups.Add(new DamagePopup
        {
            isPlayerTarget = false,
            target = target,
            amount = damage,
            timeRemaining = damagePopupLifetime
        });
    }

    private void SpawnPlayerDamagePopup(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        damagePopups.Add(new DamagePopup
        {
            isPlayerTarget = true,
            target = null,
            amount = damage,
            timeRemaining = damagePopupLifetime
        });
    }

    private void UpdateDamagePopups()
    {
        for (int i = damagePopups.Count - 1; i >= 0; i--)
        {
            damagePopups[i].timeRemaining -= Time.deltaTime;
            if (damagePopups[i].timeRemaining <= 0f)
            {
                damagePopups.RemoveAt(i);
            }
        }
    }

    private void UpdateEnemyMouseSelection()
    {
        hoveredTarget = null;
        enemyScreenRects.Clear();

        if (currentState != CombatState.ChooseAttack)
        {
            RefreshAllUnitHighlights();
            return;
        }

        if (TryGetHoveredUnit(out EnemyUnit hovered))
        {
            hoveredTarget = hovered;
        }

        if (hoveredTarget != null && Input.GetMouseButtonDown(0))
        {
            SelectTarget(hoveredTarget);
        }

        RefreshAllUnitHighlights();
    }

    private IEnumerable<EnemyUnit> GetVisibleAndTargetableUnits()
    {
        if (cardSoldierA.IsAlive)
        {
            yield return cardSoldierA;
        }

        if (cardSoldierB.IsAlive)
        {
            yield return cardSoldierB;
        }

        if (!cardSoldierA.IsAlive && !cardSoldierB.IsAlive && redQueen.IsAlive)
        {
            yield return redQueen;
        }
    }

    private bool TryGetUnitScreenRect(EnemyUnit unit, out Rect screenRect)
    {
        screenRect = default;

        if (unit == null || unit.visualRoot == null || !unit.visualRoot.activeInHierarchy)
        {
            return false;
        }

        Camera cam = GetCombatCamera();
        if (cam == null)
        {
            return false;
        }

        SpriteRenderer[] renderers = unit.visualRoot.GetComponentsInChildren<SpriteRenderer>(false);
        if (renderers.Length == 0)
        {
            return false;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 min = cam.WorldToScreenPoint(bounds.min);
        Vector3 max = cam.WorldToScreenPoint(bounds.max);
        if (min.z < 0f && max.z < 0f)
        {
            return false;
        }

        float xMin = Mathf.Min(min.x, max.x);
        float xMax = Mathf.Max(min.x, max.x);
        float yMin = Mathf.Min(min.y, max.y);
        float yMax = Mathf.Max(min.y, max.y);

        screenRect = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        return true;
    }

    private bool TryGetHoveredUnit(out EnemyUnit hoveredUnit)
    {
        hoveredUnit = null;

        Camera cam = GetCombatCamera();
        if (cam == null)
        {
            return false;
        }

        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        Vector2 worldPoint = new Vector2(mouseWorld.x, mouseWorld.y);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPoint);
        for (int i = 0; i < hits.Length; i++)
        {
            EnemyUnit hitUnit = FindUnitForCollider(hits[i]);
            if (hitUnit != null)
            {
                hoveredUnit = hitUnit;
                return true;
            }
        }

        foreach (EnemyUnit unit in GetVisibleAndTargetableUnits())
        {
            if (!TryGetUnitScreenRect(unit, out Rect rect))
            {
                continue;
            }

            if (rect.Contains(mouseScreen))
            {
                hoveredUnit = unit;
                return true;
            }
        }

        return false;
    }

    private EnemyUnit FindUnitForCollider(Collider2D hitCollider)
    {
        if (hitCollider == null)
        {
            return null;
        }

        foreach (EnemyUnit unit in GetVisibleAndTargetableUnits())
        {
            if (IsColliderForUnit(hitCollider, unit))
            {
                return unit;
            }
        }

        return null;
    }

    private static bool IsColliderForUnit(Collider2D hitCollider, EnemyUnit unit)
    {
        if (unit.targetCollider != null)
        {
            return hitCollider == unit.targetCollider;
        }

        if (unit.visualRoot == null)
        {
            return false;
        }

        Transform root = unit.visualRoot.transform;
        Transform hit = hitCollider.transform;
        return hit == root || hit.IsChildOf(root);
    }

    private Camera GetCombatCamera()
    {
        if (combatCamera != null)
        {
            return combatCamera;
        }

        return Camera.main;
    }

    private GameObject GetPlayerVisualRootObject()
    {
        if (playerVisualRoot != null)
        {
            return playerVisualRoot;
        }

        if (playerController != null)
        {
            return playerController.gameObject;
        }

        return null;
    }

    private void RefreshAllUnitHighlights()
    {
        ApplyUnitHighlight(cardSoldierA);
        ApplyUnitHighlight(cardSoldierB);
        ApplyUnitHighlight(redQueen);
    }

    private void ApplyUnitHighlight(EnemyUnit unit)
    {
        if (unit == null || unit.visualRoot == null)
        {
            return;
        }

        SpriteRenderer[] renderers = unit.visualRoot.GetComponentsInChildren<SpriteRenderer>(true);
        if (renderers.Length == 0)
        {
            return;
        }

        Color targetTint = Color.white;
        bool isSelected = unit == selectedTarget;
        bool isHovered = unit == hoveredTarget;

        if (isSelected)
        {
            targetTint = selectedTint;
        }
        else if (isHovered)
        {
            targetTint = hoverTint;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            SpriteRenderer sr = renderers[i];
            if (!originalSpriteColors.ContainsKey(sr))
            {
                originalSpriteColors[sr] = sr.color;
            }

            Color baseColor = originalSpriteColors[sr];
            sr.color = MultiplyColor(baseColor, targetTint);
        }
    }

    private static Color MultiplyColor(Color a, Color b)
    {
        return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
    }

    private void SetUnitVisible(EnemyUnit unit, bool visible)
    {
        if (unit.visualRoot != null)
        {
            unit.visualRoot.SetActive(visible);
        }
    }

    private void SetPlayerMovementLocked(bool locked)
    {
        if (playerController != null)
        {
            playerController.movementLocked = locked;
        }
    }

    private void OnGUI()
    {
        if (currentState == CombatState.Inactive)
        {
            return;
        }

        EnsureGuiResources();

        if (currentState == CombatState.ChooseAttack)
        {
            DrawActionPanel();
        }

        if (currentState == CombatState.QueenCharging || currentState == CombatState.QueenDodgeWindow)
        {
            DrawQueenPhaseUI();
        }

        if (currentState == CombatState.Victory || currentState == CombatState.Defeat)
        {
            DrawRestartButton();
        }

        DrawPlayerHealthBar();
        DrawEnemyHealthBars();
        DrawDamagePopups();
        DrawBottomMessage();
    }

    private void DrawActionPanel()
    {
        Rect attackPanel = GetAttackPanelRect();
        GUI.Box(attackPanel, GUIContent.none);
        GUI.Label(new Rect(attackPanel.x + 10f, attackPanel.y + 6f, attackPanel.width - 20f, 20f), "Attacks");
        DrawAttackButtons(attackPanel.x + 10f, attackPanel.y + 30f, attackPanel.width - 20f);

    }

    private void DrawRestartButton()
    {
        Rect attackPanel = GetAttackPanelRect();
        Rect restartRect = new Rect(attackPanel.x, attackPanel.yMax + 18f, 160f, 28f);
        if (GUI.Button(restartRect, "Restart Combat"))
        {
            InitializeEncounter();
            BeginCombat();
        }
    }

    private Rect GetAttackPanelRect()
    {
        const float width = 180f;
        const float height = 162f;

        if (TryGetPlayerScreenRect(out Rect playerRect))
        {
            float x = playerRect.xMax - 8f;
            float yTop = (Screen.height - playerRect.yMax) + 20f;
            return new Rect(x, yTop, width, height);
        }

        float fallbackCenterY = Screen.height * 0.52f;
        return new Rect(132f, fallbackCenterY - 54f, width, height);
    }

    private void DrawPlayerHealthBar()
    {
        if (!TryGetPlayerScreenRect(out Rect playerRect))
        {
            Rect fallback = new Rect(30f, Screen.height * 0.52f - 94f, playerHealthBarWidth, enemyHealthBarHeight);
            DrawHealthBar(playerHealth, playerMaxHealth, fallback);
            return;
        }

        float guiX = playerRect.center.x - (playerHealthBarWidth * 0.5f) - 30f;
        float guiY = Screen.height - playerRect.yMax - 16f;
        Rect barRect = new Rect(guiX, guiY, playerHealthBarWidth, enemyHealthBarHeight);
        DrawHealthBar(playerHealth, playerMaxHealth, barRect);
    }

    private bool TryGetPlayerScreenRect(out Rect screenRect)
    {
        screenRect = default;

        GameObject root = GetPlayerVisualRootObject();

        if (root == null || !root.activeInHierarchy)
        {
            return false;
        }

        Camera cam = GetCombatCamera();
        if (cam == null)
        {
            return false;
        }

        SpriteRenderer[] renderers = root.GetComponentsInChildren<SpriteRenderer>(false);
        if (renderers.Length == 0)
        {
            return false;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 min = cam.WorldToScreenPoint(bounds.min);
        Vector3 max = cam.WorldToScreenPoint(bounds.max);
        if (min.z < 0f && max.z < 0f)
        {
            return false;
        }

        float xMin = Mathf.Min(min.x, max.x);
        float xMax = Mathf.Max(min.x, max.x);
        float yMin = Mathf.Min(min.y, max.y);
        float yMax = Mathf.Max(min.y, max.y);

        screenRect = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        return true;
    }

    private void DrawEnemyHealthBars()
    {
        foreach (EnemyUnit unit in GetVisibleAndTargetableUnits())
        {
            if (!TryGetUnitScreenRect(unit, out Rect spriteScreenRect))
            {
                continue;
            }

            enemyScreenRects[unit] = spriteScreenRect;

            float guiX = spriteScreenRect.center.x - (enemyHealthBarWidth * 0.5f);
            float guiY = Screen.height - spriteScreenRect.yMax - 16f;
            Rect barRect = new Rect(guiX, guiY, enemyHealthBarWidth, enemyHealthBarHeight);
            DrawHealthBar(unit.currentHealth, unit.maxHealth, barRect);
        }
    }

    private void DrawBottomMessage()
    {
        Rect messageRect = new Rect(24f, Screen.height - 56f, Screen.width - 48f, 30f);
        GUI.Box(messageRect, GUIContent.none);
        GUI.Label(new Rect(messageRect.x + 8f, messageRect.y + 6f, messageRect.width - 16f, 20f), battleMessage);
    }

    private void DrawAttackButtons(float x, float y, float width)
    {
        float buttonHeight = 34f;

        for (int i = 0; i < attacks.Length; i++)
        {
            Rect buttonRect = new Rect(x, y + (buttonHeight + 7f) * i, width, buttonHeight);
            bool isSelected = selectedAttackIndex == i;

            Color previous = GUI.color;
            if (isSelected)
            {
                GUI.color = new Color(0.85f, 1f, 0.88f);
            }

            if (GUI.Button(buttonRect, (i + 1) + "  " + attacks[i].attackName))
            {
                StartPlayerAttack(i);
            }

            GUI.color = previous;
        }
    }

    private void DrawHealthBar(int current, int max, Rect barRect)
    {
        float ratio = max <= 0 ? 0f : Mathf.Clamp01((float)current / max);
        DrawSolidRect(barRect, healthBackColor);
        DrawSolidRect(new Rect(barRect.x, barRect.y, barRect.width * ratio, barRect.height), healthFillColor);
    }

    private void DrawDamagePopups()
    {
        for (int i = 0; i < damagePopups.Count; i++)
        {
            DamagePopup popup = damagePopups[i];
            if (!TryGetPopupTargetRect(popup, out Rect targetRect))
            {
                continue;
            }

            float t = 1f - Mathf.Clamp01(popup.timeRemaining / Mathf.Max(0.01f, damagePopupLifetime));
            float alpha = 1f - t;
            float rise = Mathf.Lerp(0f, 28f, t);

            float iconWidth = Mathf.Max(32f, targetRect.width * damageIconScaleOnTarget);
            float iconHeight = Mathf.Max(32f, targetRect.height * damageIconScaleOnTarget);
            float guiX = targetRect.center.x - (iconWidth * 0.5f);
            float guiY = Screen.height - targetRect.yMax - damageIconVerticalOffset - rise;
            Rect iconRect = new Rect(guiX, guiY, iconWidth, iconHeight);

            GUI.color = new Color(1f, 1f, 1f, alpha);
            if (damageIcon != null)
            {
                GUI.DrawTexture(iconRect, damageIcon, ScaleMode.ScaleToFit, true);
            }

            damageNumberStyle.normal.textColor = new Color(damageTextColor.r, damageTextColor.g, damageTextColor.b, alpha);
            Rect textRect = new Rect(iconRect.center.x - 35f, iconRect.yMax + damageNumberYOffset, 70f, 24f);
            GUI.Label(textRect, "-" + popup.amount, damageNumberStyle);
            GUI.color = Color.white;
        }
    }

    private bool TryGetPopupTargetRect(DamagePopup popup, out Rect targetRect)
    {
        targetRect = default;

        if (popup.isPlayerTarget)
        {
            return TryGetPlayerScreenRect(out targetRect);
        }

        if (popup.target == null)
        {
            return false;
        }

        if (enemyScreenRects.TryGetValue(popup.target, out targetRect))
        {
            return true;
        }

        return TryGetUnitScreenRect(popup.target, out targetRect);
    }

    private void EnsureGuiResources()
    {
        if (solidTexture == null)
        {
            solidTexture = new Texture2D(1, 1);
            solidTexture.SetPixel(0, 0, Color.white);
            solidTexture.Apply();
        }

        if (damageNumberStyle == null)
        {
            damageNumberStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
        }
    }

    private void DrawQueenPhaseUI()
    {
        EnsureGuiResources();

        float barX = (Screen.width - queenDodgeBarWidth) * 0.5f;
        float barY = Screen.height * 0.35f;

        if (currentState == CombatState.QueenCharging)
        {
            float progress = queenChargeTotal > 0f ? Mathf.Clamp01(1f - (stateTimer / queenChargeTotal)) : 1f;
            DrawSolidRect(new Rect(barX, barY, queenDodgeBarWidth, queenDodgeBarHeight), healthBackColor);
            DrawSolidRect(new Rect(barX, barY, queenDodgeBarWidth * progress, queenDodgeBarHeight), new Color(0.8f, 0.2f, 0.85f));

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(barX, barY - 22f, queenDodgeBarWidth, 20f), "Queen charging...", labelStyle);
        }

        if (currentState == CombatState.QueenDodgeWindow)
        {
            float remaining = Mathf.Clamp01(1f - (stateTimer / Mathf.Max(0.01f, queenDodgeWindowDuration)));
            DrawSolidRect(new Rect(barX, barY + queenDodgeBarHeight + 4f, queenDodgeBarWidth, queenDodgeBarHeight), healthBackColor);
            DrawSolidRect(new Rect(barX, barY + queenDodgeBarHeight + 4f, queenDodgeBarWidth * (1f - remaining), queenDodgeBarHeight), new Color(0.95f, 0.6f, 0.1f));

            GUIStyle dodgeStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 36,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 0.25f, 0.1f) }
            };

            float alpha = 1f - remaining * 0.35f;
            Color prev = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, alpha);
            GUI.Label(new Rect(0f, barY - 50f, Screen.width, 50f), "DODGE!", dodgeStyle);
            GUI.color = prev;
        }
    }

    private void DrawSolidRect(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, solidTexture, ScaleMode.StretchToFill);
        GUI.color = previous;
    }
}
