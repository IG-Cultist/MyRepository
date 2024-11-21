<?php

namespace App\Models;

use Illuminate\Contracts\Auth\Authenticatable;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class User extends Model
{
    use HasFactory;

    protected $guarded = [ //更新しないカラムを指定 idなどのauto_incrementのついているもの
        'id'
    ];

    public function items()
    {
        return $this->belongsToMany(
            Item::class, 'user_items',
            'user_id',
            'item_id')
            ->withPivot('item_num');
    }

    public function battles()
    {
        return $this->hasOne(BattleMode::class);
    }
}
