<?php

namespace App\Http\Controllers;

use App\Models\followLog;
use App\Models\ItemLog;
use App\Models\mailLog;

class LogController extends Controller
{
    public function showItemLog()
    {
        return view('logs.item_log', ['items' => ItemLog::All()]);
    }

    public function showFollowLog()
    {
        return view('logs.follow_log', ['items' => FollowLog::All()]);
    }

    public function showMailLog()
    {
        return view('logs.mail_log', ['items' => MailLog::All()]);
    }
}
