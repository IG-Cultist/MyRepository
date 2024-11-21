<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Social extends Model
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
}
