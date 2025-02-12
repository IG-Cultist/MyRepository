# カゲフミ
MagicOnionとUnityを用いて制作された作品  
1分で影踏み遊びをし、相手を倒すかHPの高い方の勝ちとなるマルチプレイ専用ゲーム  
マスタークライアント方式で他プレイヤーとの同期を実現  
HTTP/2での効率的な通信を実現　RPCを用いり開発の負担を軽減  

開発期間:2024/12/02～2025/1/30
![ゲームのスクリーンショット000](https://github.com/IG-Cultist/MyRepository/blob/main/Images/Title.png)
![ゲームのスクリーンショット001](https://github.com/IG-Cultist/MyRepository/blob/main/Images/Sample000.png)
![ゲームのスクリーンショット002](https://github.com/IG-Cultist/MyRepository/blob/main/Images/Sample001.png)
# URL
* [iOS](https://apps.apple.com/jp/app/id6741744450)
* 
# 使用技術
* PHP
* JavaScript
* Azure Function
* Laravel
* C#
* Docker

* # ゲームシステム全体構成図
![ゲームシステム全体構成図](https://github.com/IG-Cultist/MyRepository/blob/main/Images/System.PNG)

# フォルダ構成説明

gitフォルダ\  
├ 01_Admin\            # アドミンフォルダ  
│├ 01_app\             # 今作のスクリーンショットの格納先  
│└ 02_db\              # データベースのファイル格納先  
│  
├ 01_Document\         # シーケンス図格納先  
│├ 01_Attack.puml      # 攻撃のシーケンス図  
│├ 02_GameEnd.puml     # ゲーム終了のシーケンス図  
│├ 03_GameStart.puml   # ゲーム開始のシーケンス図  
│├ 04_Item.puml        # アイテムのシーケンス図  
│├ 05_Join.puml        # 参加のシーケンス図  
│├ 06_Leave.puml       # 退室のシーケンス図  
│├ 07_Matching.puml    # マッチングのシーケンス図  
│├ 08_Move.puml        # 移動のシーケンス図  
│├ 09_SetTrap.puml     # トラップ設置のシーケンス図  
│└ 10_TimeUp.puml      # タイムアップのシーケンス図  
│  
├ 03_Images            # 今作のスクリーンショットの格納先  
│├ 01_Title.png        # 今作のタイトルのスクリーンショット  
│├ 02_Sample000.png    # 今作のタイトルのスクリーンショット  
│└ 03_Sample001.png    # 今作のタイトルのスクリーンショット  
│  
├ 04_MagicOnionClient  # クライアントフォルダ  
│ └ 01_Assets\                          # ゲームに使用されるファイルの格納先(記載の必要のあるもののみ抜粋)  
│  ├ 01_Animations\                     # 各ゲームオブジェクトのアニメーションの格納先  
│  ├ 02_Free Skyboxes - Terrestrial\    # アセットストアからダウンロードしたスカイボックスのテクスチャが入ったフォルダ  
│  ├ 03_Joystick Pack\                  # アセットストアからダウンロードしたゲーム内の操作で使用する仮想ジョイスティックのフォルダ  
│  ├ 04_Materials\                      # ゲーム内のオブジェクトで使用されているマテリアの格納先  
│  ├ 05_Prefabs\                        # ゲーム内で生成されるゲームオブジェクトの格納先  
│  ├ 06_Resources\                      # ゲーム内で読み込まれるゲームオブジェクトの格納先  
│  ├ 07_Scenes\                         # ゲームで使用されるシーンの格納先  
│  ├ 08_Scripts\                        # ゲームの処理をしているスクリプトの格納先  
│  ├ 09_Simple Scene Fade Load System\  # アセットストアからダウンロードしたシーン遷移のフェード関連  
│  ├ 10_SimplePoly City\                # アセットストアからダウンロードした背景オブジェクトのフォルダ  
│  ├ 11_Sounds\                         # ゲームで使用されている効果音格納先  
│  └ 12_Textures\                       # ゲームで使用されているテクスチャ格納先  
│  
├ 05_Server               # サーバフォルダ  
│├ 01_Model/Context\      # DBモデルの格納先  
││ └ 01_GameDBContext.cs  # DB関連スクリプト  
│├ 02_Properties\         # コンテナレジストリの設定情報の格納先  
│├ 03_Services\           # 各サービスの格納先 
││ └ 01_UseService.cs     # ユーザ関連スクリプト  
│└ 04_StreamingHubs\      # ストリーミングハブフォルダ  
│  ├ 01_RoomData.cs       # ルーム情報スクリプト  
│  └ 02_RoomHub.cs        # 同期処理関連スクリプト  
│  
├ 06_Shared            # Sharedフォルダ  
│  
├ .gitignore           # ソースをコミットする際に無視するファイル、フォルダを記載しているファイル  
│  
└ 08_README.md         # 現在閲覧しているファイル  
