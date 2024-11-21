<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('accounts', function (Blueprint $table) {
            $table->id();                                 //idカラム
            $table->string('name', 20);      //nameカラム(20文字) length指定なし->255文字
            $table->string('password', 100); //パスワード(100文字)
            $table->timestamps();                         //created_at,updated_at

            //$table->index('name');        //インデックス設定
            $table->unique('name'); //ユニーク制約設定 自動でindexも付く
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('accounts');
    }
};
