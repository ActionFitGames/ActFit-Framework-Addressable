Addressable System
===
![GitHub release](https://img.shields.io/github/v/release/ActionFitGames/ActFit-Framework-Addressable.svg)

**ActionFit Organization, Dev Team - Framework**
* 보다 효율적으로 `비동기`를 활용하는 어드레서블 매니지먼트 시스템
* `KVP(Key-Value-Pair)`를 Integer와 IResourceLocation을 활용
* 액션(델리게이트)을 통해 전체 통합 프로그레스 콜백 지원
* 한 프레임에 대한 가중치 최소화 `Optimized Frame Calcuate`
* 미리 매핑되어 있는 상수 사용, 매직 넘버 최소

<br>

# Table of Contents
* [Getting Started](#getting-started)
* [Install via git URL](#install-via-git-url)
* [Start Manual](#start-manual)
* [Methods](#methods)
* [Editor Settings](#editor-settings)
* [Editor Menu](#editor-menu)
* [Editor Files](#editor-files)
* [Advanced]
  * [Editor Mechanism]

<br>

Getting Started
---
Dependency install via [GitDependencyResolver](https://github.com/hibzzgames/Hibzz.DependencyResolver.git)

어드레서블 시스템을 사용하기 위해서는 리졸버 패키지를 필요로합니다.
  1. 유니티 프로젝트 오픈
  2. Windows -> Package Manager -> 좌측 `+` 버튼
  3. Package add with Git URL (via)

```
https://github.com/hibzzgames/Hibzz.DependencyResolver.git
```
위 주소를 복사 후 `Add` 버튼을 눌러 해당 패키지 추가
**해당 내용은 사용 전 반드시 완료가 되어 있어야만 하는 작업입니다.**

<br>

Install via git URL
---
> Alert: 리졸버는 반드시 다운을 받아야만 합니다.
> [Getting Started](#getting-started)

유니티 버전은 `22.3.xx` 버전이 이상이어야만 동작합니다.

![image](https://user-images.githubusercontent.com/46207/79450714-3aadd100-8020-11ea-8aae-b8d87fc4d7be.png)

패키지 매니저에 접근하여 다음 깃 주소를 추가하여 설치합니다.<br>
설치 도중 의존성인 `UniTask`를 설치하라고 알림이 발생하면 `Install`을 해줍니다.

```
https://github.com/ActionFitGames/ActFit-Framework-Addressable.git
```

<br>

Start Manual
---
Start Use Script (활성화를 위한 사용 방법)<br>
1. `Use No Sequence` - 기존에 로딩 시퀀스가 존재하지 않는 경우
```csharp
private void Awake()
{
  AddressableSystem.Activate();
}

private void OnEnable()
{
  LocalAction = LoadedSequenceInvoke;
}

private async void Start()
{
  var loader = AddressableSystem.Instance.GetLoadProcessor();

  await loader.LoadAssetsAsync(에셋 레이블 레퍼런스(AssetLabelReference));

  LocalAction.Invoke();
}
```

2. `Use Sequence` - 로딩 시퀀스가 존재하는 경우
``` csharp
private async void Start()
{
  AddressableSystem.Activate();

  var loader = AddressableSystem.Instance.GetLoadProcessor();

  await loader.LoadAssetsAsync(에셋 레이블 레퍼런스(AssetLabelReference));

  await Otehr.Sequence();

  // ... ETC
}
```

<br>

Methods
---
> 접근 및 사용이 가능한 메서드 모음입니다.

### Addressable System
``` csharp
AddressableSystem.Activate();
// AddressableSystem.Instance를 제공합니다.
// 어드레서블 시스템 활성화를 시킵니다.

AddressableSystem.Instance.GetLoadProcessor();
// 어드레서블을 로드하는 프로세서입니다.
// LoadAssetsAsync를 제공합니다.

AddressableSystem.Instance.GetAssetProvider();
// 로드한 에셋들을 제공해줍니다. 
// 인스턴시에이트 또한 제공합니다.

AddressableSystem.Instance.GetReleaseProcessor();
// 프로바이더를 통해 제공된 게임오브젝트 및
// 메모리에 적재된 리소스를 해제하는 메서드를 제공하는 프로세서입니다.
```

### ILoadProcessor
``` csharp
LoadAssetsAsync(AssetLabelReference, Action<float> Progress, Action<Object> OnCallbackLoaded, Action OnCallbackComplete)
// 레이블 단위로 에셋을 로드할 수 있는 메서드입니다.
// Param
// AssetLabelReference - [SerializeField]를 통해 제공되는 레이블 레퍼런스입니다.
// Action<float> onProgress - 0f ~ 1f로 제공됩니다.
// Action<Object> onCallbackLoaded - 레이블 단위에서 하나의 콜백이 완료될 때마다 호출됩니다.
// Action onCallbackComplete - 레이블 단위가 완료되었을 때 호출됩니다.

LoadAssetsAsync(List<AssetLabelReference>, Action<float>, Action<Object>, Action)
// 위 메서드와 동일하지만 단일 레이블이 아닌 여러 레이블을 처리합니다.
// 실제 프로그레스 동작 방식도 가중치를 부여해 총량에 비례해서 진행됩니다.
```

### IReleaseProcessor
``` csharp
Release(AssetLabelReference)
// 메모리에 적재된 에셋을 레이블 단위로 해제합니다.

Release(List<AssetLabelReference>)
// 메모리에 적재된 에셋을 멀티플-레이블 단위로 해제합니다. (주로 씬 단위)

Release(int addressableKey)
// 메모리에 적재된 에셋 중 단일 핸들에 대하여 해제합니다.

ReleaseInstance(GameObject instance)
// Destroy + ReferenceCounting Subtract
// InstantiateAsync로 생성된 게임오브젝트를 해제합니다.

ReleaseAllInstances(int addressableKey)
// 단일 핸들을 참조하여 생성된 모든 게임오브젝트를 해제합니다.

ReleaseWithInstance(AssetLabelReference)
// 레이블 단위로 인스턴스를 포함하여 해제합니다.

ReleaseWithInstance(List<AssetLabelReference>)
// 멀티플-레이블(주로 씬 단위)로 전체 메모리를 인스턴스 포함 해제합니다.

ReleaseWithInstance(int addressableKey)
// 게임 오브젝트 인스턴스 포함 핸들까지 메모리에서 해제합니다.
```

### IAssetProvider
``` csharp
T GetAsset<T>(int addressableKey)
// GameObject를 제외한 에셋들을 반환합니다.
// FBX, Prefab과 같은 렌더러가 붙은 오브젝트를 호출할 경우 Null을 반환합니다.
// 해제된 리소스에 접근해도 Null을 반환합니다.

Instantiate(int addressableKey, Vector3 position, Quaternion rotation, Transform parent = null)
// 핸들이 유효할 경우 어드레서블 키에 해당하는 게임오브젝트를 생성 및 반환합니다.

Instantiate(int addressableKey, Transform parent = null, bool instantiateInWorldSpace = false)
// 핸들이 유효할 경우 어드레서블 키에 해당하는 게임오브젝트를 생성 및 반환합니다.
```

<br>

Editor Settings
---
에디터에서 사용가능한 어드레서블 시스템 `글로벌` 세팅에 대한 섹션입니다.

![imgur](https://imgur.com/dFasb6M.png)
> Assets/AddressableSystemSetting.asset

세팅은 위 경로에 존재합니다. 클릭하면 다음과 같은 세팅이 있습니다.
![imgur](https://imgur.com/iWJnmuW.png)

`Exception Handle Type`
예외를 처리하는 방식으로 총 3가지가 존재하며 에러를 어떻게 처리할지 타입을 정합니다. (기본값은 Log입니다.)
* Log - Debug.Log 방식
* Throw - 예외가 발생했을 때 던져 게임이 정지됩니다.
* Suppress - Obsolete, 현재 사용하고있지 않습니다.

`Is Debug`
디버그 방식이 로그일 때 활성화 여부입니다.
* 비활성화 시 프로덕션 빌드에 알맞습니다.
* 활성화 시 모든 것이 로그에 레코딩을 하기에 부하가 존재합니다.

`Use Dont Destroy On Load`
어드레서블 시스템(헬퍼, 모노 오브젝트)을 영구적인 씬으로 넘길 것에 대한 문구입니다.<br>
멀티 씬과 같이 영구적인 씬이 별도로 존재할 경우 체크할 필요가 없습니다.

`Is Auto Update`
* 어드레서블 셋팅 레이블 추가/삭제
* 엔트리(에셋)에 대한 레이블 변경사항
> 위 경우에 대해 자동 업데이트가 적용됩니다.
> 에디터 상에서 부하가 존재할 수 있으며, 불편할 경우 체크 해제 후 수동으로 [메뉴](#editor-menu)에서 처리합니다.

<br>

Editor Menu
---
오토 업데이트가 없을 때 매뉴얼로 진행할 때 또는 ForcedUpdate가 필요할 때 사용합니다.
![imgur](https://imgur.com/HUYQGue.png)

> 각 메뉴는 순차적으로 진행해야 오류가 발생하지 않습니다.
> 1-Step => 2-Step => 3-Step

1. `KeyValueData with Json` 어드레서블에 매핑하기 위한 Json파일을 만듭니다.
2. `Addressables Key Constant` 어드레서블 Json 파일을 토대로 AddrKey 리터럴 상수 키를 생성합니다.
3. `Addressables Cache`, 가장 중요한 작업으로 실제 매핑 캐시 데이터를 생성합니다.
