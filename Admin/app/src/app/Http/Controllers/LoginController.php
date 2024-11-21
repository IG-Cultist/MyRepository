<?php

namespace App\Http\Controllers;

use App\Models\Account;
use Illuminate\Http\Request;
use Illuminate\Routing\Route;
use Illuminate\Support\Facades\Hash;

class LoginController extends Controller
{
    public function index()
    {
        return view('login.login');
    }

    public function dologin(Request $request)
    {
        $validated = $request->validate([
            'name' => ['required', 'min:4', 'max:20'],
            'password' => ['required']
        ]);

        $account = Account::where('name', '=', $request['name'])->get();

        if ($account->count() === 0) {
            return redirect()->route('login', ['error' => 'invalid']);
        }

        if (Hash::check($request['password'], $account[0]['password'])) {
            $request->session()->put('login', true);
            return redirect()->route('accounts.index', ['nowUser' => $request['name']]);
        } else {
            return redirect()->route('login', ['error' => 'invalid']);
        }
    }

    public function dologout(Request $request)
    {
        $request->session()->forget('login');
        return redirect()->route('login');
    }
}
