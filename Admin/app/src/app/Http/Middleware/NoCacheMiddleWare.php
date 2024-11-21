<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

class NoCacheMiddleWare
{
    public function handle(Request $request, Closure $next): Response
    {
        $responce = $next($request);
        $responce->withHeaders(['Cache-Control' => 'no-store', 'Max-Age' => 0]);
        return $responce;
    }
}
