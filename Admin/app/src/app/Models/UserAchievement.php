<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class UserAchievement extends Model
{
    protected $guarded = [ //更新しないカラムを指定 idなどのauto_incrementのついているもの
        'id'
    ];
}
