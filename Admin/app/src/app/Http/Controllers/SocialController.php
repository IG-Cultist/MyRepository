<?php

namespace App\Http\Controllers;

use App\Models\Follow;
use App\Models\Social;
use Illuminate\Http\Request;

class SocialController extends Controller
{
    public function index()
    {
        return view('socials.social');
    }

    public function list()
    {
        $data = Follow::paginate(10);
        return view('socials.list', ['items' => $data]);
    }

    public function show(Request $request)
    {
        $data = Social::select('socials.id', 'users.name as user_name', 'socials.locate', 'socials.last_login')
            ->join('users', 'socials.user_id', '=', 'users.id')
            ->where('user_id', '=', $request->search)
            ->get();
        # フォロー数
        $followCnt = Follow::where('user_id', '=', $request->search)->count();
        # フォロワー数
        $followerCnt = Follow::where('follow_id', '=', $request->search)->count();

        return view('socials.social', ['items' => $data, 'follow' => $followCnt, 'follower' => $followerCnt]);
    }
}
