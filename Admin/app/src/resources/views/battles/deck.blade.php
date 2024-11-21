@extends('layouts.app')
@section('title','デッキ情報')
@section('battle','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <form method="post" action="{{route('battles.showDeck',['id' =>'search'])}}">
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
                            <th>ユーザーid</th>
                            <th>カードID</th>
                        </tr>
                        </thead>
                        @foreach($user as $item)
                            <tr>
                                <td>{{$item->id}}</td>
                                <td>{{$item->user_id}}</td>
                                <td>{{$item->card_id}}</td>
                            </tr>
                        @endforeach
                    </table>
                @endif
            </div>
        </div>
    </div>
@endsection
