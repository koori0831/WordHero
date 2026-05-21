using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work.Players.Code;
using Work.Weapons.Code;

namespace Work.MainEntrance.Code
{
    /// <summary>
    /// 선택된 런 시작 무기를 플레이어에게 지급하는 컴포넌트.
    /// </summary>
    public sealed class StarterLoadoutApplier : MonoBehaviour
    {
        [SerializeField] private Player targetPlayer;
        [SerializeField] private bool clearStateAfterApply = true;

        /// <summary>
        /// 컴포넌트 배치 시 시작 무기 지급 시도.
        /// </summary>
        private void Start()
        {
            if (targetPlayer == null)
            {
                targetPlayer = FindFirstObjectByType<Player>();
            }

            ApplyToPlayer(targetPlayer, clearStateAfterApply);
        }

        /// <summary>
        /// 선택된 시작 무기 2개를 플레이어에게 지급.
        /// </summary>
        /// <param name="player">지급 대상 플레이어.</param>
        /// <param name="clearAfterApply">지급 후 런 선택 상태 초기화 여부.</param>
        /// <returns>지급 성공 여부.</returns>
        public static bool ApplyToPlayer(Player player, bool clearAfterApply)
        {
            if (player == null || !RunLoadoutState.IsComplete)
            {
                return false;
            }

            BaseWeapon primaryWeapon = InstantiateWeapon(RunLoadoutState.PrimaryOption);
            BaseWeapon secondaryWeapon = InstantiateWeapon(RunLoadoutState.SecondaryOption);

            if (primaryWeapon == null || secondaryWeapon == null)
            {
                Debug.LogWarning("Starter loadout apply failed: weapon prefab instantiate failed.");
                return false;
            }

            // PlayerWeaponInventory는 마지막 지급 무기를 현재 무기로 두기 때문에 보조 무기부터 지급
            player.GetWeapon(secondaryWeapon);
            player.GetWeapon(primaryWeapon);

            if (clearAfterApply)
            {
                RunLoadoutState.Clear();
            }

            return true;
        }

        /// <summary>
        /// 시작 무기 항목의 프리팹 인스턴스 생성.
        /// </summary>
        /// <param name="option">생성할 시작 무기 항목.</param>
        /// <returns>생성된 무기 인스턴스.</returns>
        private static BaseWeapon InstantiateWeapon(StarterWeaponOption option)
        {
            if (!option.IsAvailable)
            {
                return null;
            }

            BaseWeapon weapon = Instantiate(option.WeaponPrefab);
            weapon.gameObject.SetActive(true);
            return weapon;
        }
    }

    /// <summary>
    /// InGameScene 로드 후 시작 무기를 자동 지급하는 런타임 연결자.
    /// </summary>
    internal static class StarterLoadoutAutoApplier
    {
        private const string IN_GAME_SCENE_NAME = "InGameScene";

        private static bool _isSubscribed;

        /// <summary>
        /// 씬 로드 이벤트 구독 초기화.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (_isSubscribed)
            {
                return;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            _isSubscribed = true;
        }

        /// <summary>
        /// 인게임 씬 로드 시 시작 무기 지급 예약.
        /// </summary>
        /// <param name="scene">로드된 씬.</param>
        /// <param name="mode">씬 로드 모드.</param>
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != IN_GAME_SCENE_NAME || !RunLoadoutState.IsComplete)
            {
                return;
            }

            ApplyAfterSceneReadyAsync().Forget();
        }

        /// <summary>
        /// 씬 오브젝트 초기화 이후 플레이어에 시작 무기 지급.
        /// </summary>
        private static async UniTaskVoid ApplyAfterSceneReadyAsync()
        {
            await UniTask.Yield();

            Player player = Object.FindFirstObjectByType<Player>();
            if (player == null)
            {
                Debug.LogWarning("Starter loadout apply failed: player not found in InGameScene.");
                return;
            }

            StarterLoadoutApplier.ApplyToPlayer(player, true);
        }
    }
}
