@extends('layouts.app')
@section('title','新規登録')
@section('create','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <form method="post" action="{{route('accounts.docreate')}}">
                    @csrf
                    <label for="name">ユーザネーム:</label><input class="form-control" type="text" name="name"
                                                                  id="name"><br>
                    <label for="password">パスワード:</label><input class="form-control" type="password" name="password"
                                                                    id="password"><br>
                    <label for="repassword">パスワード再入力:</label><input class="form-control" type="password"
                                                                            name="repassword"
                                                                            id="repassword"><br>
                    <label for="button"></label><input class="btn btn-info" type="submit" name="button" id="button"
                                                       value="登録">
                    <ul>
                        @if(request()->get('error') === 'alreadyExists')
                            <li>すでに同じ名前が使われています</li>
                        @elseif(request()->get('error')  === 'noMatch')
                            <li>再入力したパスワードと一致しません</li>
                        @endif
                        @if($errors->any())
                            @foreach($errors->all() as $error)
                                <li>{{$error}}</li>
                            @endforeach
                        @endif
                    </ul>
                </form>
            </div>
        </div>
    </div>
@endsection
