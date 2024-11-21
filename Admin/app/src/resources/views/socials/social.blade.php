@extends('layouts.app')
@section('title','ソーシャル')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <form method="post" action="{{route('socials.show',['id' =>'search'])}}">
                    @csrf
                    <label for="search"><input class="form-control" type="search" name="search" id="search"
                                               placeholder="ユーザidを入力"></label>
                    <input class="btn btn-info" type="submit" value="検索">
                </form>
                @if(isset($items))
                    <table>
                        <thead>
                        <tr>
                            <th>ID</th>
                            <th>ユーザ名</th>
                            <th>フォロー数</th>
                            <th>フォロワー数</th>
                            <th>所在地</th>
                            <th>最終ログイン</th>
                        </tr>
                        </thead>
                        @foreach($items as $item)
                            <tr>
                                <td>{{$item['id']}}</td>
                                <td>{{$item['user_name']}}</td>
                                <td>{{$follow}}</td>
                                <td>{{$follower}}</td>
                                <td>{{$item['locate']}}</td>
                                <td>{{$item['last_login']}}</td>
                            </tr>
                        @endforeach
                    </table>
                @endif
            </div>
        </div>
    </div>
@endsection
