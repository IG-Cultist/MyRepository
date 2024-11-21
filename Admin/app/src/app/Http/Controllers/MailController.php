<?php

namespace App\Http\Controllers;

use App\Models\Mail;
use App\Models\UserMail;
use App\Models\User;
use Illuminate\Http\Request;

class MailController extends Controller
{
    public function index(Request $request)
    {
        $data = Mail::All();
        return view('mails.mail', ['mails' => $data]);
    }

    public function send(Request $request)
    {
        return view('mails.send');
    }

    public function sendMail(Request $request)
    {
        if ($request->user_id === "ALL") { #全送信
            # メールテーブルから入力したIDの情報を取得
            $isMailExist = Mail::where('id', '=', $request['mail_id'])->get();

            # そのIDのメールが存在しない場合、再入力
            if ($isMailExist->count() === 0) {
                return redirect()->route('create', ['error' => 'notMailExists']);
            }

            $users = User::All();
            foreach ($users as $item) { #各情報をテーブルに挿入
                UserMail::create(['user_id' => $item['id'], 'mail_id' => $request['mail_id'], 'isOpen' => false]);
            }


        } else { #個別送信
            # メールテーブルから入力したIDの情報を取得
            $isMailExist = Mail::where('id', '=', $request['mail_id'])->get();

            # ユーザテーブルから入力したIDの情報を取得
            $isUserExist = User::where('id', '=', $request['user_id'])->get();

            # そのIDのメールが存在しない場合、再入力
            if ($isMailExist->count() === 0) {
                return redirect()->route('create', ['error' => 'notMailExists']);
            }

            # そのIDのユーザが存在しない場合、再入力
            if ($isUserExist->count() === 0) {
                return redirect()->route('create', ['error' => 'notUserExists']);
            }

            # 各情報をテーブルに挿入
            UserMail::create(['user_id' => $request['user_id'], 'mail_id' => $request['mail_id'], 'isOpen' => false]);
        }

        # 完了ページへリダイレクト
        return redirect()->route('mails.send', ['send' => 'success']);
    }
}
