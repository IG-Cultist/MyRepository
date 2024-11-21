<?php

namespace App\Http\Controllers;

use App\Models\User;
use Illuminate\Http\Request;

class UserItemController extends Controller
{
    #プレイヤー一覧を表示
    public function index(Request $request)
    {
        //$data = UserItem::select('user_items.id', 'users.name as user_name', 'items.name as item_name',
        //    'item_num')
        //    ->join('users', 'user_items.user_id', '=', 'users.id')
        //    ->join('items', 'user_items.item_id', '=', 'items.id')
        //    ->get();

        return view('userItem');
    }

    public function show(Request $request)
    {
        $data = User::find($request->search);
        return view('userItem', ['user' => $data]);
    }
}
