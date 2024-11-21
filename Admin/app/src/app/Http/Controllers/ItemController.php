<?php

namespace App\Http\Controllers;

use App\Models\Item;
use Illuminate\Http\Request;

class ItemController extends Controller
{
    #アカウント一覧を表示
    public function index(Request $request)
    {
        if (!$request->session()->exists('login')) {
            return redirect()->route('login');
        }
        $data = Item::All();
        return view('item', ['accounts' => $data]);
    }
}
