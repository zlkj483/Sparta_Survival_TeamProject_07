Sparta Survival Team Project 07

유니티 기반 생존형 팀 프로젝트, 2022 버전

🎯 개요

이 프로젝트는 유니티(Unity) 엔진을 활용한 생존(Survival)형 팀 프로젝트입니다.
플레이어가 재료를 수집하고, 아이템을 제작(crafting)하며, 다양한 도전 요소를 극복해 나가는 게임플레이 구조를 목표로 합니다.

🚀 주요 기능

재료 수집 및 인벤토리 관리

아이템 제작(Crafting) 시스템

레시피(ScriptableObject 기반) 적용

플레이어 입력으로 제작 메뉴 열고 닫기

UI 구성: 재료 보유 패널, 제작 가능 아이템 리스트, 상세 정보 패널

확장 가능하고 모듈화된 설계로 다른 기능 추가 용이

🧩 아키텍처 개요

CraftRecipe : 제작 레시피 데이터를 담는 ScriptableObject

CraftingUI.cs : 제작 메뉴 전체 관리 (재료 표시, 리스트 생성, 상세 정보 컨트롤)

CraftingListButton.cs : 제작가능 리스트에서 각 버튼 기능 담당

ShowCraftMenu.cs : 입력에 따라 제작 메뉴 창의 열림/닫힘 제어

UIInventory (추정) : 인벤토리 및 재료 수량 조회 / 제거 / 추가 기능 담당

🎨 UI 구성

좌측: 재료 보유 현황 (Wood, Stone 등)

우측 상단: 제작 가능 아이템 리스트 (아이콘 + 이름)

우측 하단: 선택된 아이템 상세 정보 (아이콘, 이름, 설명, 타입, 제작버튼)

버튼 및 리스트는 ScrollView, LayoutGroup 등을 활용해 유동적 구현

🔧 설치 및 실행 방법

저장소를 클론 또는 다운로드

git clone https://github.com/zlkj483/Sparta_Survival_TeamProject_07.git


Unity 2022.3.62f2 (또는 유사 버전)로 프로젝트 오픈

필요한 패키지 / 플러그인 설치 (예: TextMeshPro 등)

씬(Scene) 설정 확인: CraftingPanel UI, 인벤토리 시스템 등이 정상 연결돼 있는지 확인

플레이 모드에서 제작 메뉴 열고 재료/아이템 수량 및 제작 동작 테스트

🧠 개발 팁 / 확장 제안

레시피 데이터(CraftRecipe)를 추가해 다양한 제작 아이템 확장 가능

인벤토리 슬롯, 드래그 & 드롭 UI 구현하면 UX 향상

제작 시 애니메이션 또는 효과음 추가해 몰입감 강화

재료가 많은 경우 좌측 패널 대신 팝업 리스트 형태로 전환 고려

제작 실패 확률, 품질 등 더 복잡한 시스템도 추가 가능

📁 디렉토리 구조
Assets/
 ├── Scripts/
 │    ├── CraftingUI.cs  
 │    ├── CraftingListButton.cs  
 │    ├── ShowCraftMenu.cs  
 │    └── CraftRecipe.cs  
 ├── ScriptableObjects/
 │    └── Recipes/  
 ├── UI/
 │    ├── Panels/  
 │    └── Prefabs/  
 └── …  
Packages/  
ProjectSettings/  
.gitignore  

👥 팀 정보

작성자: zlkj483

참여 개발자: kdk7992-sketch, kimdonggwan, whoman4233 (기여자 목록)

📄 라이선스

별도 라이선스 파일이 없는 경우 기본적으로 “모든 권리 보유(All rights reserved)” 또는 원하는 오픈소스 라이선스 (MIT, Apache2 등)로 변경 가능

프로젝트에 맞게 라이선스 추가하는 것을 권장합니다.