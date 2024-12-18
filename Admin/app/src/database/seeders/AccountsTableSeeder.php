<?php

namespace Database\Seeders;

use App\Models\Account;
use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Hash;

class AccountsTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        Account::create([
            'name' => 'jobi',
            'password' => Hash::make('jobi'),
        ]);

        Account::create([
            'name' => 'IGCIGC',
            'password' => Hash::make('231075'),
        ]);

        Account::create([
            'name' => 'tera',
            'password' => Hash::make('231076'),
        ]);

        Account::create([
            'name' => 'Toufu',
            'password' => Hash::make('Goma'),
        ]);

        Account::create([
            'name' => 'cancel',
            'password' => Hash::make('01101001'),
        ]);

        Account::create([
            'name' => 'meeper',
            'password' => Hash::make('500error'),
        ]);
    }
}
