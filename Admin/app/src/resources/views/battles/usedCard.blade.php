@extends('layouts.app')
@section('title','使用済みカード情報')
@section('battle','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <form method="post" action="{{route('battles.showUsedCard',['id' =>'search'])}}">
                    @csrf
                    <label for="search"><input class="form-control" type="search" name="search" id="search"
                                               placeholder=""></label>
                    <input class="btn btn-info" type="submit" value="検索">
                </form>

                @if(isset($user))
                    <table>
                        <thead>
                        <tr>
                            <th>ID</th>
                            <th>ユーザID</th>
                            <th>カードID</th>
                            <th>使用枚数</th>
                        </tr>
                        </thead>
                        @foreach($user as $item)
                            <tr>
                                <td>{{$item->id}}</td>
                                <td>{{$item->user_id}}</td>
                                <td>{{$item->card_id}}</td>
                                <td>{{$item->stack}}</td>
                            </tr>
                        @endforeach
                    </table>
                @endif
            </div>
        </div>
    </div>
@endsection
