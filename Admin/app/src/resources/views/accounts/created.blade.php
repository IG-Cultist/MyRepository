@extends('layouts.app')
@section('title','登録完了')
@section('create','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <form method="get" action="{{route('accounts.index')}}">
                    @csrf
                    <h2>アカウント登録完了</h2>
                    <label for="button"></label><input class="btn btn-info" type="submit" name="button" id="button"
                                                       value="アカウント一覧へ">
                </form>
            </div>
        </div>
    </div>
@endsection
