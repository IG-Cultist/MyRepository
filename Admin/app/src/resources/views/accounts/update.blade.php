@extends('layouts.app')
@section('title','パスワード更新')
@section('index','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <form method="post" action="{{route('accounts.edit',['id' =>$request])}}">
                    @csrf
                    <label for="password">
                        パスワード:
                        <input class="form-control" type="password" name="password" id="password">
                    </label><br>
                    <label for="password">
                        パスワード再入力:
                        <input class="form-control" type="password"
                               name="password_confirmation" id="password_confirmation">
                    </label><br>
                    <label for="button">
                        <input class="btn btn-info" type="submit" name="button" id="button">
                    </label>
                    @if($errors->any())
                        <ul>
                            @foreach($errors->all() as $error)
                                <li>{{$error}}</li>
                            @endforeach
                        </ul>
                    @endif
                </form>
            </div>
        </div>
    </div>
@endsection
