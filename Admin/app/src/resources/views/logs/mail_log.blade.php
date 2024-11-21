@extends('layouts.app')
@section('title','メールログ')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <table>
                    <thead>
                    <tr>
                        <th>ID</th>
                        <th>ユーザID</th>
                        <th>メールID</th>
                        <th>act[0:受信/1:開封]</th>
                    </tr>
                    </thead>

                    @foreach($items as $item)
                        <tr>
                            <td>{{$item['id']}}</td>
                            <td>{{$item['user_id']}}</td>
                            <td>{{$item['mail_id']}}</td>
                            <td>{{$item['action']}}</td>
                        </tr>
                    @endforeach
                </table>
            </div>
        </div>
    </div>
@endsection
