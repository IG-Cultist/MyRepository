<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class UserMail extends Model
{
    use HasFactory;

    protected $guarded = [ //更新しないカラムを指定 idなどのauto_incrementのついているもの
        'id'
    ];
}
