@extends('layouts.app')
@section('title','アイテムログ')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <table>
                    <thead>
                    <tr>
                        <th>ID</th>
                        <th>ユーザID</th>
                        <th>アイテムID</th>
                        <th>増減値</th>
                    </tr>
                    </thead>

                    @foreach($items as $item)
                        <tr>
                            <td>{{$item['id']}}</td>
                            <td>{{$item['user_id']}}</td>
                            <td>{{$item['item_id']}}</td>
                            <td>{{$item['item_fluctuation']}}</td>
                        </tr>
                    @endforeach
                </table>
            </div>
        </div>
    </div>
@endsection
