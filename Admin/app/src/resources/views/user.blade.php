@extends('layouts.app')
@section('title','ユーザ一覧')
@section('user','true')
@section('master','false')
@section('account','false')
@section('logs','false')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                {{$accounts->onEachSide(1)->links('vendor.pagination.bootstrap-5')}}
                <table>
                    <thead>
                    <tr>
                        <th>ID</th>
                        <th>名前</th>
                        <th>レベル</th>
                        <th>経験値</th>
                        <th>ライフ</th>
                    </tr>
                    </thead>
                    @foreach($accounts as $account)
                        <tr>
                            <td>{{$account['id']}}</td>
                            <td>{{$account['name']}}</td>
                            <td>{{$account['level']}}</td>
                            <td>{{$account['exp']}}</td>
                            <td>{{$account['life']}}</td>
                        </tr>
                    @endforeach
                </table>
                {{$accounts->onEachSide(1)->links('vendor.pagination.bootstrap-5')}}
            </div>
        </div>
    </div>
@endsection
