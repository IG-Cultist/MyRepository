<?php

namespace App\Http\Controllers;

use App\Models\User;
use Illuminate\Http\Request;

class UserController extends Controller
{
    #プレイヤー一覧を表示
    public function index(Request $request)
    {
        if (!$request->session()->exists('login')) {
            return redirect()->route('login');
        }
        //$data = User::All();
        $data = User::paginate(10);
        return view('user', ['accounts' => $data]);
    }
}
