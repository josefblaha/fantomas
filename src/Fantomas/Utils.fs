﻿namespace Fantomas

open System
open System.Text
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module Char =
    let escape c =
        match c with
        | '\r' -> @"\r"
        | '\n' -> @"\n"
        | _ -> c.ToString()

[<RequireQualifiedAccess>]
module String =
    let normalizeNewLine (str : string) =
        str.Replace("\r\n", "\n").Replace("\r", "\n")

    let normalizeThenSplitNewLine (str : string) =
        (normalizeNewLine str).Split('\n')

    let startsWithOrdinal (prefix : string) (str : string) =
        str.StartsWith(prefix, StringComparison.Ordinal)

    let removeTrailingSpaces (source:string) =
        source.Split([| Environment.NewLine |], StringSplitOptions.None)
        |> Array.map (fun line -> line.TrimEnd())
        |> fun lines -> String.Join(Environment.NewLine, lines)
        |> fun code -> code.TrimStart(Environment.NewLine.ToCharArray())
        
    let private lengthWithoutSpaces (str: string) =
        normalizeNewLine str
        |> fun s -> s.Replace("\n", String.Empty)
        |> String.length

    let private hashRegex = @"^\s*#(if|elseif|else).*"
    let private splitWhenHash (source: string) = 
        source.Split([| Environment.NewLine; "\r\n"; "\n" |], options = StringSplitOptions.None)
        |> Array.fold (fun acc line ->
            if Regex.IsMatch(line, hashRegex) then
                match acc with
                | [[]] -> [[line]]
                | _ -> [line]::acc
            else
                acc
                |> List.mapi (fun idx l -> if idx = 0 then (line::l) else l)
        ) [[]]
        |> List.map (List.rev >> String.concat Environment.NewLine)
        |> List.rev

    let merge a b =
        let aChunks = splitWhenHash a
        let bChunks = splitWhenHash b
        List.zip aChunks bChunks
        |> List.map (fun (a', b') ->
            if lengthWithoutSpaces a' > lengthWithoutSpaces b' then a' else b'
        )
        |> String.concat Environment.NewLine


    let escape (str: string) =
        let builder = StringBuilder()
        let result =
            let chars =
                str.ToCharArray()
                |> Array.indexed
            let totalChars = Array.length chars

            let isFollowedBySlashN currentIndex =
                let nextIndex = currentIndex + 1
                if nextIndex = totalChars then
                    false
                else
                    '\n' = (snd chars.[nextIndex])

            let isPrecededBySlashR currentIndex =
                if currentIndex = 0 then
                    false
                else
                    let prevIndex = currentIndex - 1
                    '\r' = (snd chars.[prevIndex])

            chars
            |> Array.fold (fun (b:StringBuilder, insideString) (i,c) ->
                match c, insideString with
                | ''', false ->
                    b.Append("'"), false
                | '"', _ ->
                    b.Append("\\\""), not insideString
                | ''', _ ->
                    b.Append("\\\""), not insideString
                | '\n', false when (isPrecededBySlashR i) ->
                    b,insideString
                | '\n', false ->
                    b.Append("\\n"), false
                | '\r', false when (isFollowedBySlashN i) ->
                    b.Append("\n"), false
                | '\r', false ->
                    b.Append("\\r"), false
                | c, _ ->
                    b.Append(c), insideString
            ) (builder, false)
            |> fst
            |> fun b -> b.ToString()

        result


module Cache =
    let alreadyVisited<'key when 'key : not struct>() =
        let cache = System.Collections.Generic.HashSet<'key>([], HashIdentity.Reference)
        fun key ->
            if cache.Contains key then
                true
            else
                cache.Add key |> ignore
                false

module Dict =
    let tryGet k (d: System.Collections.Generic.IDictionary<_,_>) =
        let (r,x) = d.TryGetValue k
        if r then Some x else None
        
module List =
    let appendItem l i =
        l @ [i]
        
    let prependItem l i = i :: l
    
    let takeWhileState f state l =
        let mutable s = state
        l |> List.takeWhile (fun x -> let (s',r) = f s x in s <- s'; r)