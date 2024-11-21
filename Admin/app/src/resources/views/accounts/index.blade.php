@extends('layouts.app')
@section('title','アカウント一覧')
@section('index','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <form method="post" action="{{url('accounts/show')}}">
                    @csrf
                    <label for="search"><input class="form-control" type="search" name="search" id="search"
                                               placeholder="名前を入力"></label>
                    <input class="btn btn-info" type="submit" value="検索">
                </form>
                <ul>
                    @if(!empty(request()->get('deleted')))
                        <li>{{request()->get('deleted')}}の削除に成功</li>
                    @elseif(!empty(request()->get('updated')))
                        <li>{{request()->get('updated')}}のパスワードを更新</li>
                    @endif
                </ul>
                {{$accounts->links()}}
                <table>
                    <thead>
                    <tr>
                        <th>ID</th>
                        <th>名前</th>
                        <th>パスワードハッシュ</th>
                        <th>編集</th>
                    </tr>
                    </thead>
                    @foreach($accounts as $account)
                        <tr>
                            <td>{{$account['id']}}</td>
                            <td>{{$account['name']}}</td>
                            <td>{{$account['password']}}</td>
                            <td>
                                <form method="get" action="{{route('accounts.update',['id' =>$account['id']])}}">
                                    @csrf
                                    <input class="btn btn-info" type="submit" value="変更">
                                </form>

                                @if($nowUser !== $account['name'])
                                    <form method="post" action="{{route('accounts.destroy',['id' =>$account['id']])}}">
                                        @csrf
                                        <input class="btn btn-info" type="submit" value="削除">
                                    </form>
                                @endif
                            </td>
                        </tr>
                    @endforeach
                </table>
                {{$accounts->links()}}
            </div>
        </div>
    </div>
@endsection
