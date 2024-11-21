@extends('layouts.app')
@section('title','戦歴情報')
@section('battle','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <form method="post" action="{{route('battles.showResult',['id' =>'search'])}}">
                    @csrf
                    <label for="search"><input class="form-control" type="search" name="search" id="search"
                                               placeholder="勝利ユーザidを入力"></label>
                    <input class="btn btn-info" type="submit" value="検索">
                </form>

                @if(isset($user))
                    <table>
                        <thead>
                        <tr>
                            <th>ID</th>
                            <th>勝者ID</th>
                            <th>敗者ID</th>
                        </tr>
                        </thead>
                        @foreach($user as $item)
                            <tr>
                                <td>{{$item->id}}</td>
                                <td>{{$item->winner_id}}</td>
                                <td>{{$item->loser_id}}</td>
                            </tr>
                        @endforeach
                    </table>
                @endif
            </div>
        </div>
    </div>
@endsection
