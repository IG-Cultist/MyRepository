@extends('layouts.app')
@section('title','所有アイテム一覧')
@section('userItem','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <form method="post" action="{{route('userItems.show',['id' =>'search'])}}">
                    @csrf
                    <label for="search"><input class="form-control" type="search" name="search" id="search"
                                               placeholder="ユーザidを入力"></label>
                    <input class="btn btn-info" type="submit" value="検索">
                </form>
                @if(isset($user))
                    <table>
                        <thead>
                        <tr>
                            <th>ID</th>
                            <th>ユーザー名</th>
                            <th>アイテム名</th>
                            <th>所持個数</th>
                        </tr>
                        </thead>
                        @foreach($user->items as $item)
                            <tr>
                                <td>{{$item['id']}}</td>
                                <td>{{$user->name}}</td>
                                <td>{{$item->name}}</td>
                                <td>{{$item->pivot->item_num}}</td>
                            </tr>
                        @endforeach
                    </table>
                @endif
            </div>
        </div>
    </div>
@endsection
