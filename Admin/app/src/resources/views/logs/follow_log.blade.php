@extends('layouts.app')
@section('title','フォローログ')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <table>
                    <thead>
                    <tr>
                        <th>ID</th>
                        <th>ユーザID</th>
                        <th>フォロー先ユーザID</th>
                        <th>act[0:解除/1:登録]</th>
                    </tr>
                    </thead>

                    @foreach($items as $item)
                        <tr>
                            <td>{{$item['id']}}</td>
                            <td>{{$item['user_id']}}</td>
                            <td>{{$item['target_user_id']}}</td>
                            <td>{{$item['action']}}</td>
                        </tr>
                    @endforeach
                </table>
            </div>
        </div>
    </div>
@endsection
