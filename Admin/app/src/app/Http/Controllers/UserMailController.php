<?php

namespace App\Http\Controllers;

use App\Models\UserMail;
use Illuminate\Http\Request;

class UserMailController extends Controller
{
    #========================
    # メール受信者一覧画面
    #========================
    public function index(Request $request)
    {
        if (!$request->session()->exists('login')) {
            return redirect()->route('login');
        }
        // 必要なカラムを取得
        $data = UserMail::select('user_mails.id', 'users.name as user_name', 'mails.text as mail_txt',
            'items.name as item_name', 'mails.item_num', 'user_mails.isOpen')
            ->join('users', 'user_mails.user_id', '=', 'users.id')
            ->join('mails', 'user_mails.mail_id', '=', 'mails.id')
            ->join('items', 'mails.item_id', '=', 'items.id')
            ->get();
        return view('userMail', ['accounts' => $data]);
    }
}
