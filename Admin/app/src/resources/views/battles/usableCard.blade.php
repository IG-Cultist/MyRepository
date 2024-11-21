@extends('layouts.app')
@section('title','使用可能カード情報')
@section('battle','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                @if(isset($card))
                    <table>
                        <thead>
                        <tr>
                            <th>ID</th>
                            <th>名称</th>
                            <th>種別</th>
                            <th>枚数</th>
                        </tr>
                        </thead>
                        @foreach($card as $item)
                            <tr>
                                <td>{{$item->id}}</td>
                                <td>{{$item->name}}</td>
                                <td>{{$item->type}}</td>
                                <td>{{$item->stack}}</td>
                            </tr>
                        @endforeach
                    </table>
                @endif
            </div>
        </div>
    </div>
@endsection
