@extends('layouts.app')
@section('title','アイテム一覧')
@section('item','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <table>
                    <thead>
                    <tr>
                        <th>ID</th>
                        <th>名前</th>
                        <th>効果値</th>
                        <th>説明</th>
                    </tr>
                    </thead>

                    @foreach($accounts as $account)
                        <tr>
                            <td>{{$account['id']}}</td>
                            <td>{{$account['name']}}</td>
                            <td>{{$account['effect']}}</td>
                            <td>{{$account['explain']}}</td>
                        </tr>
                    @endforeach
                </table>
            </div>
        </div>
    </div>
@endsection
