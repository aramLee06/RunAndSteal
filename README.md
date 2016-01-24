# Game-RunAndSteal
Run and Steal

## Build

빌드하고자 하는 종류에 따라 'File' - 'Build Settings' 에서 Scene을 다르게 선택함

___BigScreen Game (약칭 BS)___

-	SPLASH/BS_LogoViewer
-	[Scene1]Lobby/LobbyHost
-	[Scene2]CharacterSelectLobby/CharacterSelectLobbyBS
-	[Scene3]Tutorial/TutorialBS_Android
-	[Scene3]Tutorial/InGameBS
-	[Scene4]GameLoading/GameLoadingBS
-	[Scene5]InGame/InGameBS
-	[Scene6]Result/ResultBS

___Pad Controller (약칭 PS)___

-	[Scene1]Lobby/LobbyClient
-	[Scene2]CharacterSelectLobby/ChararcterSelectLobbyPS
-	[Scene3]Tutorial/TutorialPS
-	[Scene5]InGame/InGamePS
-	[Scene6]Result/ResultPS


### Game Build

위의 항목에 따라 Scene을 선택 후, 플랫폼 선택하여 빌드

### Pad Build

상단 메뉴에서 선택

`‘ONEPAD’ – ‘AssetBundle’ – ‘Make Asset Bundle’`

프로젝트의 루트폴더에 *.unity3d 파일이 생성됨
