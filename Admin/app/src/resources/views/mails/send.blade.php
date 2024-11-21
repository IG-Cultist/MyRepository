@extends('layouts.app')
@section('title','メール送信')
@section('send','active')
@section('body')
    <div class="container">
        <div class="row">
            <div class="col-md-6">
                <ul>
                    @if(!empty(request()->get('send')))
                        <li>送信完了</li>
                    @endif
                </ul>
                <form method="post" action="{{route('mails.sendMail')}}">
                    @csrf
                    <label for="user_id">ユーザID(全ユーザに送る場合は"ALL"と入力)</label>
                    <input class="form-control" type="text" name="user_id" id="user_id"
                           placeholder="送信先ユーザID"><br>

                    <label for="mail_id">メールID</label>
                    <input class="form-control" type="text" name="mail_id"
                           id="mail_id" placeholder="送信するメールID"><br>

                    <label for="button"></label>
                    <input class="btn btn-info" type="submit" name="button"
                           id="button" value="送信">

                </form>
            </div>
        </div>
    </div>
@endsection
