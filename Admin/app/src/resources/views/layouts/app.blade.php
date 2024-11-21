<html lang="ja" data-bs-theme="dark">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>@yield('title')</title>
    <link href="/css/bootstrap.min.css" rel="stylesheet"
          integrity="sha384-9ndCyUaIbzAi2FUVXJi0CjmCapSmO7SnpJef0486qhLnuZ2cdeRhO02iuK6FUUVM" crossorigin="anonymous">
    <link rel="stylesheet" href="/app.css">
    <link rel="stylesheet" href="/css/style.css">
</head>
<body>
<header class="d-flex flex-wrap justify-content-center py-3 mb-4 border-bottom">
    <a class="d-flex align-items-center mb-3 mb-md-0 me-md-auto link-body-emphasis text-decoration-none">
        <svg class="bi me-2" width="40" height="32">
            <use xlink:href="#bootstrap"></use>
        </svg>
        <span class="fs-4">@yield('title')</span>
    </a>
</header>
<main class="d-flex flex-nowrap">
    <div class="flex-shrink-0 p-3" style="width: 280px;">

        <ul class="list-unstyled ps-0">
            <li class="mb-1">
                <button class="btn btn-toggle d-inline-flex align-items-center rounded border-0 collapsed"
                        data-bs-toggle="collapse" data-bs-target="#home-collapse" aria-expanded="@yield('master')">
                    MasterData
                </button>
                <div class="collapse show" id="home-collapse">
                    <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                        <li>
                            <form method="get" action="{{route('accounts.index')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="管理者">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('item.index')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="アイテム">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('mails.send')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="メール送信">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('mails.index')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="メールテンプレート">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('battles.usableCard')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="使用可能カード">
                            </form>
                        </li>
                    </ul>
                </div>
            </li>
            <li class="mb-1">
                <button class="btn btn-toggle d-inline-flex align-items-center rounded border-0 collapsed"
                        data-bs-toggle="collapse" data-bs-target="#dashboard-collapse" aria-expanded="@yield('user')">
                    UserData
                </button>
                <div class="collapse" id="dashboard-collapse">
                    <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                        <li>
                            <form method="get" action="{{route('user.index')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="ユーザー">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('userItems.index')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="所持アイテム">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('userMail.index')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="メール受信者">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('socials.social')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="ソーシャル">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('socials.list')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="フォローリスト">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('battles.index')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="バトルモード">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('battles.deck')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="デッキ情報">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('battles.result')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="戦歴情報">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('battles.defenseDeck')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="防衛デッキ情報">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('battles.usedCard')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="使用済みカード情報">
                            </form>
                        </li>
                    </ul>
                </div>
            </li>

            <li class="mb-1">
                <button class="btn btn-toggle d-inline-flex align-items-center rounded border-0 collapsed"
                        data-bs-toggle="collapse" data-bs-target="#account-collapse" aria-expanded="@yield('account')">
                    Account
                </button>
                <div class="collapse" id="account-collapse">
                    <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                        <li>
                            <form method="get" action="{{route('accounts.create')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="Create Account">
                            </form>
                        </li>
                        <li>
                            <form method="post" action="{{route('login.logout')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="Sign out">
                            </form>
                        </li>
                    </ul>
                </div>
            </li>

            <li class="mb-1">
                <button class="btn btn-toggle d-inline-flex align-items-center rounded border-0 collapsed"
                        data-bs-toggle="collapse" data-bs-target="#log-collapse" aria-expanded="@yield('log')">
                    Log
                </button>
                <div class="collapse" id="log-collapse">
                    <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                        <li>
                            <form method="get" action="{{route('logs.itemLog')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="ItemLog">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('logs.mailLog')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="MailLog">
                            </form>
                        </li>
                        <li>
                            <form method="get" action="{{route('logs.followLog')}}">
                                @csrf
                                <input class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                       type="submit" value="FollowLog">
                            </form>
                        </li>
                    </ul>
                </div>
            </li>
        </ul>
    </div>
    <div class="b-example-divider b-example-vr"></div>
    @yield('body')</main>
<script src="/js/bootstrap.bundle.min.js"
        integrity="sha384-geWF76RCwLtnZ8qwWowPQNguL3RmwHVBC9FhGdlKrxdiJJigb/j/68SIy3Te4Bkz"
        crossorigin="anonymous"></script>
</body>
</html>
