<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

class AuthMiddleware
{
    public function handle(Request $request, Closure $next): Response
    {
        if (!$request->session()->exists('login')) {
            return redirect()->route('login');
        }
        $responce = $next($request);
        return $responce;
    }
}
