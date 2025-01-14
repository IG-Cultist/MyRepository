<?php

namespace Database\Seeders;

use App\Models\User;
use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class UsersTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        User::create([
            'name' => 'jobi',
            'token' => '',
            'skin_name' => 'shadow_normal',
        ]);

        User::create([
            'name' => 'igc',
            'token' => '',
            'skin_name' => 'shadow_normal',
        ]);

        User::create([
            'name' => 'tera',
            'token' => '',
            'skin_name' => 'shadow_normal',
        ]);

        User::create([
            'name' => 'meep',
            'token' => '',
            'skin_name' => 'shadow_normal',
        ]);

        User::create([
            'name' => 'morsh',
            'token' => '',
            'skin_name' => 'shadow_normal',
        ]);
    }
}
