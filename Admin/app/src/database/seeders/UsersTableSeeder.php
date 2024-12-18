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
        ]);
        
        User::create([
            'name' => 'igc',
            'token' => '',
        ]);

        User::create([
            'name' => 'tera',
            'token' => '',
        ]);

        User::create([
            'name' => 'meep',
            'token' => '',
        ]);

        User::create([
            'name' => 'morsh',
            'token' => '',
        ]);
    }
}
