<?php

namespace App\Http\Controllers;

use App\Models\Account;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Hash;

class AccountController extends Controller
{
    #========================
    # アカウント一覧画面
    #========================
    public function index(Request $request)
    {
        $nowUser = $request->nowUser;
        //$accounts = Account::All();
        $accounts = Account::simplePaginate(10); # 指定値以下の場合、表示されない
        return view('accounts.index', ['accounts' => $accounts, 'nowUser' => $nowUser]);
    }

    #========================
    # ユーザ検索処理
    #========================
    public function show(Request $request)
    {
        $accounts = Account::where('name', '=', $request['search'])->get();
        return view('accounts.index', ['accounts' => $accounts]);
    }

    #========================
    # ユーザ新規登録画面
    #========================
    public function create()
    {
        return view('accounts.create');
    }

    #========================
    # 保存完了画面
    #========================
    public function store()
    {
        return view('accounts/created');
    }

    #========================
    # ユーザ新規登録処理
    #========================
    public function docreate(Request $request)
    {
        $validated = $request->validate([
            'name' => ['required', 'min:4', 'max:20'],
            'password' => ['required']
        ]);

        # アカウントテーブルから入力されたnameを取得
        $account = Account::where('name', '=', $request['name'])->get();

        # すでにその名前が存在していた場合、再入力
        if ($account->count() !== 0) {
            return redirect()->route('create', ['error' => 'alreadyExists']);
        }

        # パスワードと再入力したパスワードが異なる場合、再入力
        if ($request['password'] !== $request['repassword']) {
            return redirect()->route('accounts.create', ['error' => 'noMatch']);
        }

        # 名前とハッシュ化したパスワードを登録
        Account::create(['name' => $request['name'], 'password' => Hash::make($request['password'])]);

        # 完了ページへリダイレクト
        return view('accounts.created');
    }

    #========================
    # ユーザ消去処理
    #========================
    public function destroy(Request $request)
    {
        $account = Account::findOrFail($request->id);
        $account->delete();

        return redirect()->route('accounts.index', ['deleted' => $account->name]);
    }

    #========================
    # PW更新処理
    #========================
    public function edit(Request $request)
    {
        $request->validate([
            'password' => ['required', 'confirmed']
        ]);

        $account = Account::findOrFail($request->id);
        $account->password = Hash::make($request->password);
        $account->save();

        return redirect()->route('accounts.index', ['updated' => $account->name]);
    }

    #========================
    # PW更新画面
    #========================
    public function update(Request $request)
    {
        return view('accounts.update', ['request' => $request->id]);
    }
}
