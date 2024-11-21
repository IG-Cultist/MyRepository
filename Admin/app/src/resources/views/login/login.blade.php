<html lang="ja" data-bs-theme="dark">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>ログイン</title>
    <link href="/css/bootstrap.min.css" rel="stylesheet"
          integrity="sha384-9ndCyUaIbzAi2FUVXJi0CjmCapSmO7SnpJef0486qhLnuZ2cdeRhO02iuK6FUUVM" crossorigin="anonymous">
    <link rel="stylesheet" href="/app.css">
</head>
<body>
<div class="container">
    <div class="row">
        <div class="col-md-6">
            <main class="form-signin w-100 m-auto">
                <form method="post" action="{{route('login.login')}}">
                    @csrf
                    <img class="mb-4" src="/Floppa.png" alt="" width="72" height="57">
                    <h1 class="h3 mb-3 fw-normal">Please sign in</h1>

                    <div class="form-floating">
                        <input class="form-control" type="text" name="name" id="floatingInput" placeholder="Name">
                        <label for="floatingInput">Name</label>
                    </div>
                    <div class="form-floating">
                        <input class="form-control" type="password" name="password" id="floatingPassword"
                               placeholder="Password">
                        <label for="floatingPassword">Password</label>
                    </div>

                    <button class="btn btn-primary w-100 py-2" type="submit" name="button" id="button">Sign in</button>
                    @if($errors->any())
                        <ul>
                            @foreach($errors->all() as $error)
                                <li>{{$error}}</li>
                            @endforeach
                        </ul>
                    @endif
                </form>
            </main>
        </div>
    </div>
</div>
<script src="/js/bootstrap.bundle.min.js"
        integrity="sha384-geWF76RCwLtnZ8qwWowPQNguL3RmwHVBC9FhGdlKrxdiJJigb/j/68SIy3Te4Bkz"
        crossorigin="anonymous"></script>
</body>
</html>
