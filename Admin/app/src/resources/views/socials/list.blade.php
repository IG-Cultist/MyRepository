@extends('layouts.app')
@section('title','フォローリスト')
@section('user','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                {{$items->onEachSide(1)->links('vendor.pagination.bootstrap-5')}}
                <table>
                    <thead>
                    <tr>
                        <th>ID</th>
                        <th>ユーザID</th>
                        <th>フォローID</th>
                    </tr>
                    </thead>
                    @foreach($items as $item)
                        <tr>
                            <td>{{$item['id']}}</td>
                            <td>{{$item['user_id']}}</td>
                            <td>{{$item['follow_id']}}</td>
                        </tr>
                    @endforeach
                </table>
                {{$items->onEachSide(1)->links('vendor.pagination.bootstrap-5')}}
            </div>
        </div>
    </div>
@endsection
