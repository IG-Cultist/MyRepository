<?php

use App\Http\Controllers\AccountController;
use App\Http\Controllers\BattleController;
use App\Http\Controllers\LogController;
use App\Http\Controllers\LoginController;
use App\Http\Controllers\ItemController;
use App\Http\Controllers\MailController;
use App\Http\Controllers\SocialController;
use App\Http\Controllers\UserMailController;
use App\Http\Controllers\UserItemController;
use App\Http\Controllers\UserController;
use App\Http\Middleware\AuthMiddleware;
use App\Http\Middleware\NoCacheMiddleWare;
use Illuminate\Support\Facades\Route;

# キャッシュ無効化
Route::middleware(NoCacheMiddleWare::class)->group(function () {

    # アカウントコントローラーの処理をグループ化
    Route::prefix('accounts')->name('accounts.')->controller(AccountController::class)
        ->middleware(AuthMiddleware::class)->group(function () {
            Route::get('create', 'create')->name('create');          #新規作成画面
            Route::get('store', 'store')->name('store');             #作成完了画面
            Route::get('index', 'index')->name('index');             #一覧画面
            Route::get('{id}/update', 'update')->name('update');     #更新画面

            Route::post('{id}/destroy', 'destroy')->name('destroy'); #削除処理
            Route::post('{id}/edit', 'edit')->name('edit');          #編集処理
            Route::post('show', 'show')->name('show');               #検索処理
            Route::post('docreate', 'docreate')->name('docreate');   #新規作成処理
        });

    # ソーシャルコントローラの処理をグループ化
    Route::prefix('socials')->name('socials.')->controller(SocialController::class)
        ->middleware(AuthMiddleware::class)->group(function () {
            Route::get('social', 'index')->name('social');   #一覧画面
            Route::get('list', 'list')->name('list');   #フォローリスト画面
            Route::post('show', 'show')->name('show');   #検索処理
        });

    # メールコントローラの処理をグループ化
    Route::prefix('mails')->name('mails.')->controller(MailController::class)
        ->middleware(AuthMiddleware::class)->group(function () {
            # 一覧画面
            Route::get('index', 'index')->name('index');

            # 送信フォーム
            Route::get('send', 'send')->name('send');

            # 送信処理
            Route::post('sendMail', 'sendMail')->name('sendMail');
        });

    # ログインコントローラの処理をグループ化
    Route::prefix('login')->name('login.')->controller(LoginController::class)
        ->group(function () {
            # ログイン処理
            Route::post('login', 'dologin')->name('login');

            # ログアウト処理
            Route::post('logout', 'dologout')->name('logout');
        });

    # 所有アイテムコントローラの処理をグループ化
    Route::prefix('userItems')->name('userItems.')->controller(UserItemController::class)
        ->middleware(AuthMiddleware::class)->group(function () {
            # 一覧画面
            Route::get('index', 'index')->name('index');

            # 検索処理
            Route::post('{id}/show', 'show')->name('show');
        });

    # ログコントローラーの処理をグループ化
    Route::prefix('logs')->name('logs.')->controller(LogController::class)
        ->middleware(AuthMiddleware::class)->group(function () {
            Route::get('itemLog', 'showItemLog')->name('itemLog');      #アイテムログ画面
            Route::get('mailLog', 'showMailLog')->name('mailLog');      #メールログ画面
            Route::get('followLog', 'showFollowLog')->name('followLog');#フォローログ画面
        });

    # バトルコントローラの処理をグループ化
    Route::prefix('battles')->name('battles.')->controller(BattleController::class)
        ->middleware(AuthMiddleware::class)->group(function () {
            # 一覧画面
            Route::get('index', 'index')->name('index');
            # デッキ一覧画面
            Route::get('deck', 'deck')->name('deck');
            # リザルト一覧画面
            Route::get('result', 'result')->name('result');
            # 使用可能カード一覧画面
            Route::get('usableCard', 'usableCard')->name('usableCard');
            # 使用済みカード一覧画面
            Route::get('usedCard', 'usedCard')->name('usedCard');
            # 防衛デッキ一覧画面
            Route::get('defenseDeck', 'defenseDeck')->name('defenseDeck');

            # 検索処理
            Route::post('{id}/show', 'show')->name('show');
            # デッキ検索処理
            Route::post('{id}/showDeck', 'showDeck')->name('showDeck');
            # リザルト検索処理
            Route::post('{id}/showResult', 'showResult')->name('showResult');
            # 使用済みカード検索処理
            Route::post('{id}/showUsedCard', 'showUsedCard')->name('showUsedCard');
            # 防衛デッキ検索処理
            Route::post('{id}/showDefenseDeck', 'showDefenseDeck')->name('showDefenseDeck');
        });

    # ログイン画面
    Route::get('/', [LoginController::class, 'index'])->name('login');

    # プレイヤー一覧画面
    Route::get('userList', [UserController::class, 'index'])->name('user.index');

    # アイテム一覧画面
    Route::get('itemList', [ItemController::class, 'index'])->name('item.index');

    # 受信者一覧画面
    Route::get('userMail', [UserMailController::class, 'index'])->name('userMail.index');
});
